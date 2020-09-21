using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using Sandbox.Game.Gui;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public abstract class MenuItemBase
        {
            protected string Text;
            protected Action OnClick;
            protected bool IsSelected;
            protected char SelectionChar;
            protected MenuPage Parent;
            protected Menu Root;

            public virtual string RenderToString()
            {
                return (IsSelected ? SelectionChar.ToString() : "") + Text + "\n";
            }

            public virtual void HighlightThis()
            {
                foreach (var i in Parent.GetItems()) i.DehilightThis();
                IsSelected = true;
            }

            public virtual void DehilightThis()
            {
                IsSelected = false;
            }

            public virtual void Select()
            {
                OnClick();
            }
        
        }
        public class MenuItemSingle : MenuItemBase
        {

            public MenuItemSingle(string text,char selectionChar = '>')
            {
                Text = text;
                OnClick = null;
                SelectionChar = selectionChar;
            }

            public MenuItemSingle(string text, Action onClickAction, char selectionChar = '>')
            {
                Text = text;
                OnClick = onClickAction;
                SelectionChar = selectionChar;
            }

        }
        public class MenuPage : MenuItemBase
        {
            private readonly List<MenuItemBase> _items;
            private readonly string _separator;
            protected int SelectionIndex;

            public MenuPage(MenuPage parent,string title,List<MenuItemBase> items, char selectionChar = '>',char separator = '-')
            {
                Parent = parent;
                Text = title;
                _items = items;
                _separator = new string(separator,8);
                SelectionChar = selectionChar;
            }
            public override void Select()
            {
                Root.ChangePage(this);
            }

            public void Back()
            {
                Root.ChangePage(Parent);
            }
            public string RenderPage()
            {
                return Text + "\n" + _separator + "\n" + _items.Aggregate("", (current, item) => current + item.RenderToString()) + _separator;
            }

            public List<MenuItemBase> GetItems()
            {
                return _items;
            }
        }
        public class Menu : MenuPage
        {
            private MenuPage Page;
            private bool IsDirty;
            public Menu(string text,MenuPage page)
            {
                Text = text;
                Page = page;
                SelectionIndex = selectionIndex;
            }

            public override string RenderToString()
            {
                return Page.RenderPage();
            }

            public void ChangePage(MenuPage NewPage)
            {
                Page = NewPage;
                IsDirty = true;
            }

            
        }
    }
}
