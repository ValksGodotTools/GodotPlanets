﻿namespace Planets;

public class CommandDebug : Command
{
    public override void Run(string[] args)
    {
        Logger.Log("Debug");
    }
}
