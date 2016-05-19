using System.Collections;
using System.Collections.Generic;

public class AIPotentialAction
	/** Represents a potential action the AI can take during the strategic gameplay.
	 *  (I.E. this is like a prototype for the choice the AI can make).
	 *  They have the following properties:
	 * 	1. Necessary prerequisite, specified by a Condition. If this evaluates to true, the action is valid; if not, it is invalid.
	 * 	2. A base weight, representing the likelihood that this action should be chosen.
	 * 	3. AI choice weight modifiers, specified by a condition together with an integer. 
	 * 	4. The target district where the action is taking place, if any.
	 * 	5. The effect of the action:
	 * 		i. Increase faction power in the target district.
	 * 		ii. Recruit a unit in the district.
	 * 		iii. 
	 * 
	 * */
{
}
