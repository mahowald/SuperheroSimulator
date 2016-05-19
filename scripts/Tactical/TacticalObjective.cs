using UnityEngine;
using System.Collections;

public class TacticalObjective
{
    enum State
    {
        Incomplete,
        Success,
        Failure
    };

    State state = State.Incomplete;

    string objectiveTitle;

    string objectiveText;


}
