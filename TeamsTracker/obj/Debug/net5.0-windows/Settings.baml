   M S B A M L   `   `   `  ????G  CTeamsTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamsTrackerTeamsTracker  5   	       TeamsTracker.Settings   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TeamsTracker     5   	     ? Title$  Settings??5   	   $	??161??6   $	??427??6'    ? 
ResizeMode$ NoResize=?63    ? Topmost$
 True??6I    ? WindowStartupLocation$ CenterOwner=?6X   .??5	      ? .+?5
   
   ?? $H??BTeams tracker can launch another program when a teams meeting ends??6   $
??Left=?6]   $??	16,10,0,0q?6x   $	??Top=?6?   ?? 5   
   $??Excecutable:??6   $
??Left=?6'   $??	16,36,0,0q?6B   $	??Top=?6U   ?? 5   
   -   5         ??Name$	 exe??5      $
??Left=?6   $??	96,40,0,0q?6:    ?? TextWrapping$
 Wrap=?6M   $??C:/Path/To/Exe??6a   $	??Top=?6w   $	??304??6?   ?? 5   
   $'??!Arguments (Use {time} and {name})??6   $
??Left=?6<   $??	16,62,0,0q?6W   $	??Top=?6j   ?? 5   
   -   5        $
 args??5      $
??Left=?6    $??
210,66,0,0q?6;   $
 Wrap=?6O   $??-name {name} -time {time}??6c   $	??Top=?6?   $	??190??6?   ?? 5   
   -   5        $
??Save??5      $
??Left=?6    $??
371,89,0,0q?6;   $	??Top=?6O   6g   5      5      