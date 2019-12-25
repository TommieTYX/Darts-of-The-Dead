public class Player {
    public int id;
    public int numOfTurns = 3;
    public UnityEngine.GameObject icon;

    public Player(int playerId) {
        this.id = playerId;
    }

    public bool haveTurnLeft() {
        if (numOfTurns > 0) {
            return true;
        }
        return false;
    }

    public void tookATurn() {
        numOfTurns--;
    }

    public void tookAllTurns()
    {
        numOfTurns = 0;
    }

    public void endTurn() {
        numOfTurns = 3;
    }
}
