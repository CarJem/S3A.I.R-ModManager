using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace Sonic3AIR_ModManager
{

	public static class DiscordRP
	{
		//Used For Discord Rich Presence

		public static string APP_ID = "434894884391092234";
		public static System.Timers.Timer timer;
		private static bool DisableLogging = true;
		private static TimeSpan UpdateInterval { get; set; } = new TimeSpan();
		public static DateTime StartTime { get; set; } = new DateTime();

		public static DiscordRpcClient client;

		private static void StartTimer()
		{
			timer = new System.Timers.Timer(UpdateInterval.TotalMilliseconds);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			UpdateDiscord();
		}

		public static void InitDiscord()
		{
			/*
			Create a discord client
			NOTE: 	If you are using Unity3D, you must use the full constructor and define
					 the pipe connection.
			*/
			client = new DiscordRpcClient(APP_ID);
			UpdateInterval = TimeSpan.FromSeconds(5);

			//Subscribe to events
			client.OnReady += Client_OnReady;
			client.OnPresenceUpdate += Client_OnPresenceUpdate;
			client.OnError += Client_OnError;

			//Connect to the RPC
			bool status = client.Initialize();
			client.SetPresence(Presence.GetRichPresence());
			StartTimer();
		}

		private static void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
		{
			string output = string.Format("[Discord RPC] Received Ready from user {0}", args.User.Username);
			Program.Log.InfoFormat(output);
		}

		private static void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
		{
			if (!DisableLogging)
			{
				string output = string.Format("[Discord RPC] Received Update! {0}", args.Presence);
				System.Diagnostics.Debug.Print(output);
				Console.WriteLine(output);
				Program.Log.InfoFormat(output);
			}
		}

		private static void Client_OnError(object sender, DiscordRPC.Message.ErrorMessage args)
		{
			if (!DisableLogging)
			{
				string output = string.Format("[Discord RPC] Failed Update! {0}", args.Message);
				System.Diagnostics.Debug.Print(output);
				Console.WriteLine(output);
				Program.Log.InfoFormat(output);
			}
		}

		public static class Presence
		{
			/*public static RichPresence GetRichPresence()
			{

				RichPresence richPresence = new RichPresence();
				richPresence.Details = GetDetails();
				richPresence.State = GetState();
				richPresence.Timestamps = GetTimestamps();
				richPresence.Assets = GetAssets();
				return richPresence;
			}*/

			public static int LoopPoint = 0;
			public static int LoopEnd = 4;

			public static RichPresence GetRichPresence()
			{
				RichPresence richPresence = new RichPresence();
				if (LoopPoint >= LoopEnd) LoopPoint = 0;
				switch (LoopPoint)
				{
					case 0:
						GetLoop1Details(ref richPresence);
						break;
					case 1:
						GetLoop2Details(ref richPresence);
						break;
					case 2:
						GetLoop3Details(ref richPresence);
						break;
					case 3:
						GetLoop4Details(ref richPresence);
						break;

				}
				GetAssets(ref richPresence);
				GetTimestamps(ref richPresence);
				LoopPoint++;

				return richPresence;
			}

			public static void GetLoop1Details(ref RichPresence richPresence)
			{
				richPresence.Details = string.Format("[1/4] | A.I.R. Version:");
				richPresence.State = string.Format("Version {0}", MainDataModel.LastAIREXEVersion);
			}

			public static void GetLoop2Details(ref RichPresence richPresence)
			{
				richPresence.Details = "[2/4] | Status: ";
				if (GameHandler.isGameRunning)
				{
					richPresence.State = "Currently In-Game";
				}
				else
				{
					richPresence.State = "Waiting in Launcher";
				}
			}

			public static void GetLoop3Details(ref RichPresence richPresence)
			{
				richPresence.Details = "[3/4] | Mods Loaded: ";
				string count = "N/A";
				if (ModManagement.S3AIRActiveMods != null) count = ModManagement.S3AIRActiveMods.ActiveMods.Count().ToString() + " Mods";
				else count = "N/A";
				richPresence.State = count;
			}

			public static void GetLoop4Details(ref RichPresence richPresence)
			{
				richPresence.Details = string.Format("[4/4] | M.M. Version:");
				richPresence.State = string.Format("Version {0}", Program.Version);
			}


			#region Classic

			public static void GetAssets(ref RichPresence richPresence)
			{
				if (richPresence.Assets == null) richPresence.Assets = new Assets();
				richPresence.Assets.LargeImageKey = "asset4";
				if (GameHandler.isGameRunning) richPresence.Assets.SmallImageKey = "ingame";
				else richPresence.Assets.SmallImageKey = "offline";

				//richPresence.Assets.LargeImageText = "";
				//richPresence.Assets.SmallImageKey = "";
				//richPresence.Assets.SmallImageText = "";
			}

            #endregion


            #region Timestamps
            static bool timeStampSet = false;
			public static void GetTimestamps(ref RichPresence richPresence)
			{
				if (!timeStampSet)
				{
					timeStampSet = true;
					Timestamps timestamp = new Timestamps();
					timestamp.Start = Timestamps.Now.Start;
					richPresence.Timestamps = timestamp;
					StartTime = timestamp.Start.Value;
				}
				else
				{
					Timestamps timestamp = new Timestamps();
					timestamp.Start = StartTime;
					richPresence.Timestamps = timestamp;
				}

			}
            #endregion
        }

        public static void UpdateDiscord()
		{
			//Invoke all the events, such as OnPresenceUpdate
			client.SetPresence(Presence.GetRichPresence());
			client.Invoke();
		}

		public static void DisposeDiscord()
		{
			client.Dispose();
		}

	}
}
