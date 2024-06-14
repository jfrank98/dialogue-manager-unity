public class GlobalVars
{
  private static GlobalVars instance;

  // Add your global variables here
  public bool skip;
  public string str;
  private GlobalVars()
  {
    // Initialize your variables here
    skip = false;
  }

  public static GlobalVars Instance
  {
    get
    {
      instance ??= new GlobalVars();
      return instance;
    }
  }
}
