   M S B A M L   `   `   `  ????G  CTeamsTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamsTrackerTeamsTracker  5   	       TeamsTracker.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TeamsTracker     5   	     ? Title$  TeamsTracker??5   	   $	??427??6   $	??272??6+    ? Icon$! /icons8-room-finder-256.png??67   .??5	      ? .+?5
   
   ]? $??30??6   $??1??6    $	??Top=?61    K? ZIndex$ 10??6I   $??2??6[   .+?5      ?? $??
Running...??6   $??62??6)   $??10,0,0,0q?64   ?? 5      -   5        $??Settings??5      $??47??6?   $??Right=?6J   $??Center=?6f   $??0,0,10,0q?6?   5      ?? 5   
   -   5         ??Name$ outputSelector??5      $??
10,30,10,0q?6+   $??2??6?   $??20??6S   $	??Top=?6_   .@?5      ?? 
 ? Tag$ Output??6   $??Output??6(   ?? 5      $ Debug??6   $??DEBUG??6'   5      ?? 5   
   -   5         ??Name$ 
consoleOut??5       ?? AcceptsReturn$
 True??6&    ?? TextWrapping$
 Wrap=?6;   $??10,50,10,10q?6O   ${?False??6d   $??2??6v   .??6?   ??5      ?? 5      .?5      ??  ?? Property ? y?	 ?? Value$	 	Collapsed=?6=   
 ?? Triggers
 5      ?? 5       ?? Binding 5      ?? *SelectedItem.Tag+ ?? ElementName$ outputSelector?? ?? Value
 Output6Z   .a?5      ??  ? y?$	 Visible=?6E   5      5       .??5!      5"      ?? 5#   
   -   5        $ debugOut??5#      $
 True??6$   $
 Wrap=?69   $??10,50,10,10q?6M   ${?False??6c   $??2??6u   .??6?   ??5$      ?? 5%      .?5&      ??  ? y?$	 	Collapsed=?6=   
 5'      ?? 5(       5)      ?? *SelectedItem.Tag+$ outputSelector??	 Debug6Z   .a?5*      ??  ? y?$	 Visible=?6E   5+      5-      .??5.      5/      52      53      