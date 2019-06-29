namespace My_Web_Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text.RegularExpressions;
    public static class SessionManager
    {
        private const int sessionLifeSpanInMinutes = 1;
        private const string sessionCookieName = "SES_ID";
        private const string setSessionString = "Set-Cookie: SES_ID={0}; Expires={1}; HttpOnly";
        private static ConcurrentDictionary<string, Session> sessionsIdTimespans = new ConcurrentDictionary<string, Session>();

        public static bool ContainsSession(string request,string actuallSessionId)
        {
            return request.Contains(sessionCookieName)&& request.Contains(actuallSessionId);
        }

        public static string SetSesion(Session session)
        {
            return string.Format(setSessionString, session.Id, session.EndDate.ToString("R"));
        }

        public static Session ExtractSessionFromRequest(string request)
        {
            string pattern = $@"{sessionCookieName}=(?<ID>.+)(;|$)";

            string sessionId = GetSessionId(request);
            Session currentSession;
            if (string.IsNullOrEmpty(sessionId) || !sessionsIdTimespans.ContainsKey(sessionId))
            {
                //Session must be created and added to the response;
                sessionId = Guid.NewGuid().ToString();
                while (sessionsIdTimespans.ContainsKey(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                }

                currentSession = new Session(sessionId, DateTime.UtcNow, sessionLifeSpanInMinutes);

                sessionsIdTimespans[sessionId] = currentSession;

            }
            else
            {
                currentSession = sessionsIdTimespans[sessionId];
                currentSession.TimesLogedIn++;
            }
            return currentSession;
        }

        private static string GetSessionId(string input)
        {
            var cookieLine = input.Split(Environment.NewLine).FirstOrDefault(x => x.StartsWith("Cookie:"));
            if (string.IsNullOrEmpty(cookieLine))
            {
                return null;
            }
            string session = cookieLine.Replace("Cookie: ", "").Split("; ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.StartsWith(sessionCookieName));
            if (string.IsNullOrEmpty(session))
            {
                return null;
            }
            return session.Split('=', 2).Last();
        }

                         }

    public class Session
    {

        public Session(string id, DateTime firstCreated, int lifeSpanInMinutes, int timesLogedIn = 1)
        {
            Id = id;
            FirstCreated = firstCreated;
            EndDate = firstCreated.AddMinutes(lifeSpanInMinutes);
            TimesLogedIn = timesLogedIn;
        }

        public string Id { get; }
        public DateTime FirstCreated { get; set; }
        public DateTime EndDate { get; set; }
        public int TimesLogedIn { get; set; }
    }


}