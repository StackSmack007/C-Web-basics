namespace SIS.HTTP.Sessions
{
    using SIS.HTTP.Sessions.Contracts;
    using System.Collections.Generic;
    public class HttpSession : IHttpSession
    {
        public string Id { get; private set; }

        private IDictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.Id = id;
            parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string name, object parameter)
        {
            parameters[name] = parameter;
        }

        public void ClearParameters()
        {
            parameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            return parameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            if (parameters.ContainsKey(name))
            {
                return parameters[name];
            }
            return null;
        }
    }
}