using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum CommandInput
{
    idle = 0,
    forward = 1,
    backward = 2,
    left = 3,
    right = 4,
    jump = 6,
    dancing = 7
};
public class Command
{
    public uint sequence;
    public CommandInput input;
    public CommandResult result;
}

public class CommandResult
{
    public Vector3 postion;
    public Quaternion rotation;
    public Vector3 scale;

}

