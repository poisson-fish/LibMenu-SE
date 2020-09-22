# LibMenu
## A simple menu system for Space Engineers programmable blocks
LibMenu is a very simple stateful menu library I threw together to mixin with my Space Engineers MDK scripts and provide a simple user interface for them.
## Usage
Clone this repo and add it to your MDK script solution, as well as a reference to it for your MDK project. You can read more about using MDK mixin projects at Malware's MDK Github found **[here.](https://github.com/malware-dev/MDK-SE/wiki/Mixin-Projects)**

After you've added the reference to LibMenu you can define menu structures progammatically as follows.

```
var mainMenuPage = new MenuPage("Main Menu", new List<MenuItemBase>(){
                new MenuItemSingle("Option 1"),
                new MenuItemSingle("Option 2"),
                new MenuPage("Nested Menu",new List<MenuItemBase>()
                {
                    new MenuItemSingle("Nested Option 1"),
                    new MenuPage("Deep Nested Menu",new List<MenuItemBase>()
                    {
                        new MenuItemSingle("Deep Nested Option 1"),
                        new MenuItemSingle("Deep Nested Option 2"),
                        new MenuItemSingle("Deep Nested Option with Action 3",(Action)(() => Echo("I was clicked!"))),
                        new MenuBackButton()
                    }),
                    new MenuBackButton()
                })
        });
Menu aMenu = new Menu(mainMenuPage);
```
As the menu is stateful, you can navigate it with the functions defined by the Menu class.

```aMenu.Previous(); // Moves the selection cursor up the list.```\
```aMenu.Next(); // Moves the selection cursor down the list.```\
```aMenu.Back(); // Goes up a nest level.```\
```aMenu.Select(); // Selects a list item, calling its optionally defined action.```

To get a string of the menus current state, use this:\
```aMenu.RenderToString(); // Renders the menu to an easy string.```

More documentation will follow, and I'll be working on this more to make it better in my free time.
Feel free to add me on Discord, I am happy to help if you are having issues.\
twin#4486
