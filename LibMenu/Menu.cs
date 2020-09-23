/*
    LibMenu - A simple menu system for Space Engineers programmable blocks
    Copyright (C) 2020  twin (udidwhy@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    LibMenu - Copyright (C) 2020  twin (udidwhy@gmail.com)
    This program comes with ABSOLUTELY NO WARRANTY;
    This is free software, and you are welcome to redistribute it
    under certain conditions;
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public delegate void MenuItemAction(MenuItemBase b);

        public static int WrapValue(int x, int xMin, int xMax)
        {
            return (((x - xMin) % (xMax - xMin)) + (xMax - xMin)) % (xMax - xMin) + xMin;
        }
        public abstract class MenuItemBase
        {
            protected string Text;
            protected bool IsSelected;
            protected char SelectionChar;
            protected MenuPage Parent;
            protected Menu Root;
            public MenuItemAction SelectAction;

            public MenuItemBase()
            {
                SelectAction = new MenuItemAction(DefaultAction);
            }
            public virtual string RenderToString()
            {
                return (IsSelected ? SelectionChar.ToString() : "") + Text + "\n";
            }

            public virtual void HighlightThis()
            {
                Parent.UnhighlightAll();
                IsSelected = true;
            }

            public virtual void UnhighlightThis()
            {
                IsSelected = false;
            }

            public virtual void Select()
            {
                SelectAction?.Invoke(this);
            }

            public virtual Menu GetRoot()
            {
                return Root;
            }
            public virtual MenuItemBase GetParent()
            {
                return Parent;
            }
            public virtual void SetParent(MenuPage parent)
            {
                Parent = parent;
            }
            public virtual void SetRoot(Menu root)
            {
                Root = root;
            }
            public void SetText(string text)
            {
                Text = text;
                Root?.MarkDirty();
            }
            public virtual void DefaultAction(MenuItemBase b)
            {
                //Do nothing
            }
        }
        public class MenuItemSingle : MenuItemBase
        {
            public MenuItemSingle(string text, char selectionChar = '>')
            {
                Text = text;
                SelectAction = new MenuItemAction(DefaultAction);
                SelectionChar = selectionChar;
            }

            public MenuItemSingle(string text, MenuItemAction onClickAction, char selectionChar = '>')
            {
                Text = text;
                SelectAction = new MenuItemAction(onClickAction);
                SelectionChar = selectionChar;
            }

        }
        public class MenuBackButton : MenuItemBase
        {
            public MenuBackButton(char selectionChar = '>')
            {
                Text = "<-- Back";
                SelectionChar = selectionChar;
            }
            public MenuBackButton(string text, char selectionChar = '>')
            {
                Text = text;

                SelectionChar = selectionChar;
            }

            public override void Select()
            {
                Parent?.Back();
            }
        }
        public class MenuPage : MenuItemBase
        {
            private readonly List<MenuItemBase> _items;
            private readonly string _separator;
            private int _selectionIndex;

            public MenuPage(string title, List<MenuItemBase> items, char selectionChar = '>', char separator = '-', int separatorCount = 15)
            {
                _items = items;
                foreach (var item in _items) item.SetParent(this);
                Text = title;
                _separator = new string(separator, separatorCount);
                SelectionChar = selectionChar;
                _selectionIndex = 0;
                _items[_selectionIndex].HighlightThis();
            }
            public override void Select()
            {
                var item = _items[_selectionIndex];
                if (item is MenuPage)
                {
                    Root.ChangePage(_items[_selectionIndex] as MenuPage);
                }
                else
                {
                    _items[_selectionIndex].Select();
                }
            }

            public void Previous()
            {
                _selectionIndex = WrapValue(_selectionIndex - 1,0,_items.Count);
                _items[_selectionIndex].HighlightThis();
            }
            public void Next()
            {
                _selectionIndex = WrapValue(_selectionIndex + 1, 0, _items.Count);
                _items[_selectionIndex].HighlightThis();
            }
            public void Back()
            {
                if(Parent != null) //Are we already top level?
                    Root.ChangePage(Parent);
            }

            public void UnhighlightAll()
            {
                foreach (var i in _items) i.UnhighlightThis();
            }

            public string RenderPage()
            {
                return Text + "\n" + _separator + "\n" + _items.Aggregate("", (current, item) => current + item.RenderToString()) + _separator;
            }
            public override void SetRoot(Menu root)
            {
                Root = root;
                foreach (var item in _items) item.SetRoot(root);
            }
        }
        public class Menu
        {
            private MenuPage _page;
            private bool _isDirty;
            private string _stateCache;
            public Menu(MenuPage page)
            {
                _page = page;
                page.SetRoot(this);
                _stateCache = _page.RenderPage();
                _isDirty = false;
            }

            public void MarkDirty()
            {
                _isDirty = true;
            }
            public string RenderToString()
            {
                if (_isDirty)
                {
                    _isDirty = false;
                    _stateCache = _page.RenderPage();
                }

                return _stateCache;
            }

            public void ChangePage(MenuPage newPage)
            {
                _page = newPage;
                _isDirty = true;
            }
            public void Previous()
            {
                _page.Previous();
                _isDirty = true;
            }
            public void Next()
            {
                _page.Next();
                _isDirty = true;
            }

            public void Select()
            {
                _page.Select();
                _isDirty = true;
            }

            public void Back()
            {
                _page.Back();
                _isDirty = true;
            }
        }
    }
}