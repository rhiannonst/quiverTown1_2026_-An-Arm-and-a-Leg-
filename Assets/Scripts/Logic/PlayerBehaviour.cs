

using System.Security.Cryptography.X509Certificates;

class PlayerBehaviour : MonoBehaviour
{
    private Player playerSO;
    public event Action TakeDamage;

    //then a constructor for the data Layer
    public PlayerBehaviour(){
        MaxHealth = playerSO.MaxHealth;
        Name = playerSO.name;    
        RelicList = new List<Relic>();
        BaseBlock = baseBlock;
    }


    // public event Action<PlayerBehaviour> OnTakeDamage(Player){
        
    // }

    public float TakeDamage(PlayerBehaviour player)
    {
        // publisher
        TakeDamage.Invoke();
    }

    /*Most of the time these events take on the form of public event Action<[classtype], bool> [PropertyName]Changed; 
    or public event Action SomethingHappened;. 
    In these cases, there are two benefits. 
    First, I get a type for the issuing class. If MyClass declares and is the only class firing the event, I get an explicit instance of MyClass to work with in the event handler. 
    Secondly, for simple events such as property change events, the meaning of the parameters is obvious and stated in the name of the event handler and I don't have to create a myriad of classes for these kinds of events.*/
}