using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;
using Newtonsoft.Json;

namespace MusicStream
{
    /// <summary>
    /// Listener to events from SpotifySession
    /// </summary>
    public class MusicStreamSessionListener : SpotifySessionListener
    {
        private MusicStreamSessionManager _sessionManager;
        

        public MusicStreamSessionListener(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public override void NotifyMainThread(SpotifySession session)
        {
            base.NotifyMainThread(session);
            _sessionManager.InvokeProcessEvents(session);
        }

        public override void CredentialsBlobUpdated(SpotifySession session, string blob)
        {
            base.CredentialsBlobUpdated(session, blob);
            _sessionManager.CredentialsBlob = blob;
        }

        public override void LoggedIn(SpotifySession session, SpotifyError error)
        {
            if (error == SpotifyError.Ok)
            {
                base.LoggedIn(session, error);
                _sessionManager.LoggedInCallback();
            }
            else
            {
                _sessionManager.SpotifyError(error);
            }           
        }

        public override void LoggedOut(SpotifySession session)
        {
            base.LoggedOut(session);
            _sessionManager.LoggedOutCallback();
        }

        public override void UserinfoUpdated(SpotifySession session)
        {
            base.UserinfoUpdated(session);
            _sessionManager.Userdata = session.UserData;
        }

        public override void ConnectionstateUpdated(SpotifySession session)
        {
            base.ConnectionstateUpdated(session);
        }

        public override void ConnectionError(SpotifySession session, SpotifyError error)
        {
            base.ConnectionError(session, error);
            _sessionManager.SpotifyError(error);
        }

        public override void LogMessage(SpotifySession session, string data)
        {
            base.LogMessage(session, data);
            NotifyMainThread(session);
        }

        public override int MusicDelivery(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            _sessionManager.MusicDeliveryCallback(session, format, frames, num_frames);
            return num_frames;
        }

        public override void GetAudioBufferStats(SpotifySession session, out AudioBufferStats stats)
        {
            stats = _sessionManager.GetCurrentAudioBufferStats();
        }

        public override void StreamingError(SpotifySession session, SpotifyError error)
        {
            base.StreamingError(session, error);
            _sessionManager.SpotifyError(error);
        }

        public override void MetadataUpdated(SpotifySession session)
        {
            _sessionManager.Session = session;
        }

        public override void StartPlayback(SpotifySession session)
        {

        }

        public override void StopPlayback(SpotifySession session)
        {
            base.StopPlayback(session);
        }

        public override void EndOfTrack(SpotifySession session)
        {
            _sessionManager.EndOfTrack(session);
            base.EndOfTrack(session);
        }
    }
}
