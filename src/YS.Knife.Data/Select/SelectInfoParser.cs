﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace YS.Knife.Data
{
    internal class SelectInfoParser:BaseParser
    {
        public SelectInfoParser(CultureInfo currentCulture)
        {
            _currentCulture = currentCulture;
        }
        private readonly CultureInfo _currentCulture;
        public SelectInfo ParseSelectInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text);
            SelectInfo selectInfo = ParseSelectInfo(context);
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return selectInfo;
        }
        private SelectInfo ParseSelectInfo(ParseContext context)
        {
            SelectInfo selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>()
            };
            while (context.SkipWhiteSpace())
            {
                var (found, name) = context.TryParseName();
                if (found)
                {
                    selectInfo.Items.Add(ParseSelectItem(name, context));
                    if (context.SkipWhiteSpace() && context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return selectInfo;
        }
        private SelectItem ParseSelectItem(string name, ParseContext context)
        {
            SelectItem item = new SelectItem { Name = name };
            if (context.SkipWhiteSpace() && context.Current() == '{')
            {
                // parse collection infos
                ParseCollectionInfos(item, context);
            }
            if (context.SkipWhiteSpace() && context.Current() == '(')
            {
                // parse sub items
                context.Index++;
                item.SubItems = ParseSelectInfo(context).Items;
                if (context.SkipWhiteSpace() == false || context.Current() != ')')
                {
                    throw ParseErrors.MissCloseBracket(context);
                }
                else
                {
                    context.Index++;
                }
            }
            return item;
        }
        private void ParseCollectionInfos(SelectItem selectItem, ParseContext context)
        {   //skip start {
            context.Index++;
            if (context.SkipWhiteSpace())
            {
                if (context.Current() == '(')
                {
                    // parse filter info
                    ParseCollectionInfosFromFilterInfo(selectItem, context);
                }
                else if (ParseContext.IsValidNameFirstChar(context.Current()))
                {
                    int originIndex = context.Index;
                    //maybe filter,or order info
                    var (_, name) = context.TryParseName();

                    if (!context.SkipWhiteSpace())
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                    if (context.Current() == ',')
                    {
                        // order,
                        context.Index = originIndex;
                        ParseCollectionInfosFromOrderInfo(selectItem, context);

                    }
                    else if (context.Current() == '}')
                    {
                        // single order
                        //selectItem.CollectionOrder = new OrderInfo(OrderItem.Parse(name));

                    }
                    else
                    {
                        context.Index = originIndex;
                        // parse from filter
                        ParseCollectionInfosFromFilterInfo(selectItem, context);
                    }
                }
                else if (char.IsDigit(context.Current()))
                {
                    int originIndex = context.Index;
                    // limit info, or filter info
                    var (_, num) = context.TryParseUnsignInt32();

                    if (!context.SkipWhiteSpace())
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                    if (context.Current() == ',')
                    {
                        // offset,limit
                        selectItem.CollectionLimit = new LimitInfo { Offset = num };

                        context.Index++;

                        ParseCollectionLimit(selectItem.CollectionLimit, context);

                    }
                    else if (context.Current() == '}')
                    {
                        // limit
                        selectItem.CollectionLimit = new LimitInfo { Limit = num };

                    }
                    else
                    {
                        context.Index = originIndex;
                        // parse from filter
                        ParseCollectionInfosFromFilterInfo(selectItem, context);
                    }
                }
                else
                {
                    throw ParseErrors.InvalidText(context);
                }
                // skip end }
                context.SkipWhiteSpaceAndFirstChar('}');
            }
            else
            {
                throw ParseErrors.InvalidText(context);
            }
        }

        private FilterInfo2 ParseFilterInfo(ParseContext context)
        {
            return new FilterInfoParser2(this._currentCulture).ParseFilterExpression(context);
        }
        private void ParseCollectionInfosFromFilterInfo(SelectItem selectItem, ParseContext context)
        {
            selectItem.CollectionFilter = ParseFilterInfo(context);
            if (context.SkipWhiteSpace())
            {
                if (context.Current() == ',')
                {
                    context.Index++;
                    ParseCollectionInfosFromOrderInfo(selectItem, context);
                }
            }
        }
        private void ParseCollectionInfosFromOrderInfo(SelectItem selectItem, ParseContext context)
        {
           // selectItem.CollectionOrder =new OrderInfoParser(CultureInfo.CurrentCulture).ParseOrderInfo(context);

            //offset,limit
            if (context.SkipWhiteSpace())
            {
                ParseCollectionInfosFromLimitInfo(selectItem, context);


            }
        }
        private void ParseCollectionInfosFromLimitInfo(SelectItem selectItem, ParseContext context)
        {
            var (success, first) = context.TryParseUnsignInt32();
            if (success)
            {
                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                    var (success2, second) = context.TryParseUnsignInt32();
                    if (success2)
                    {
                        selectItem.CollectionLimit = new LimitInfo(first, second);
                    }
                    else
                    {
                        throw ParseErrors.ParaseLimitNumberError(context);
                    }
                }
                else
                {
                    selectItem.CollectionLimit = new LimitInfo(0, first);
                }
            }
        }
        private void ParseCollectionLimit(LimitInfo limit, ParseContext context)
        {
            if (context.SkipWhiteSpace())
            {
                var (success, num) = context.TryParseUnsignInt32();
                if (success)
                {
                    limit.Limit = num;
                }
                else
                {
                    throw ParseErrors.ParaseLimitNumberError(context);
                }
            }
            else
            {
                throw ParseErrors.ParaseLimitNumberError(context);
            }
        }
    }
}

