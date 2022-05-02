@echo off

wmic computersystem set AutomaticManagedPagefile=False

wmic pagefileset where name="C:\\pagefile.sys" set InitialSize=4096,MaximumSize=4096