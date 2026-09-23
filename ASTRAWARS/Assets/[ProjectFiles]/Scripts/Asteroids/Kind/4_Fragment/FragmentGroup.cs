using UnityEngine;
using UnityEngine.UIElements;

public class FragmentGroup
{
    private int totalExp;
    private int fragmentsLeft;

    public FragmentGroup(int exp, int count) { 
        totalExp = exp;
        fragmentsLeft = count;
    }

    public void EliminatedByPlayer(Vector3 position) { 
        fragmentsLeft--;

        if (fragmentsLeft <= 0)
            GiveBonus(position);
    }

    private void GiveBonus(Vector3 position) {
        ExpManager.GiveExp(totalExp, position);
    }
}
