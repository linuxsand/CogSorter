CogSorter is a utility that can sort CogJobs. I was confused that Cognex QuickBuild does not offer this function, that's why I made this.

Simpley run CogSorter.exe with no arguments, it will print usage message, then load \*.vpp file, input sequence that you expect, save to new file path.

    C:\> CogSorter.exe
    USAGE:
    > load xxx.vpp
    > list
    > sort 1,2,0
    > save yyy.vpp
    > exit

-----

This VS solution references Cognex VisionPro 9.0 dynamic link libraries (ver 59.0.0.0), and will be compiled to x86 assembly.

-----

Public Domain