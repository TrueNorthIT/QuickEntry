   M S B A M L   `   `   `  ???? G  CTeamsTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamsTrackerTeamsTracker      TeamsTracker.Settings   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:TeamsTracker       ? Title$  Settings??$	??161??$	??427?? ? 
ResizeMode$ NoResize=? ? Topmost$
 True?? ? WindowStartupLocation$ CenterOwner=?.??? .+??? $H??BTeams tracker can launch another program when a teams meeting ends??$
??Left=?$??	16,10,0,0q?$	??Top=??? $??Excecutable:??$
??Left=?$??	16,36,0,0q?$	??Top=??? -    ??Name$	 exe??$
??Left=?$??	96,40,0,0q? ?? TextWrapping$
 Wrap=?$??C:/Path/To/Exe??$	??Top=?$	??304???? $'??!Arguments (Use {time} and {name})??$
??Left=?$??	16,62,0,0q?$	??Top=??? -   $
 args??$
??Left=?$??
210,66,0,0q?$
 Wrap=?$??-name {name} -time {time}??$	??Top=?$	??190???? -   $
??Save??$
??Left=?$??
371,89,0,0q?$	??Top=?