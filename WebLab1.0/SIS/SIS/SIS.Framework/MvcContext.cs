namespace SIS.Framework
{
    public class MvcContext
    {
        private static MvcContext instance;

        private MvcContext() { }

        public static MvcContext Get
        {
            get
            {
                if (instance is null)
                {
                    instance = new MvcContext();
                }
                return instance;
            }
        }

public string AssemblyName { get; set; }

        public string ControllersFolder { get; set; } = "../../../Controllers";//TODOCHECK

        public string ControllerSuffix { get; set; } = "Controller";

        public string ViewsFolder { get; set; }= "Views";//TODOCHECK

        public string ModelsFolder { get; set; } = "Models";

    }
}
