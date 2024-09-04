namespace RobotCastle.Saving
{
    public interface IDataSaver
    {
	    bool DidLoad { get; }
	    void LoadSave<T>(T defaultObject);
		void SaveAll();
		T GetData<T>();
		void Delete<T>();
    }
}