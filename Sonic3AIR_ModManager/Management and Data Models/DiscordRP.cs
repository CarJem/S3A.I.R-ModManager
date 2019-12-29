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
		#region Variables

		//Used For Discord Rich Presence

		public static string APP_ID = "434894884391092234";
		public static System.Timers.Timer timer;
		public static DateTime StartTime { get; set; }

		public static DiscordRpcClient client;

        #endregion

        #region Timer

		private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			UpdateDiscord();
		}
        #endregion

        #region Event Messages

        private static void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
		{
			string output = string.Format("[Discord RPC] Received Ready from user {0}", args.User.Username);
			Program.PrintOutput(output);
		}

		private static void Client_OnClose(object sender, DiscordRPC.Message.CloseMessage args)
		{
			string output = string.Format("[Discord RPC] Connection to Client Lost [Reason: {0}]", args.Reason);
			Program.PrintOutput(output, 1);
		}

		private static void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
		{
			string output = string.Format("[Discord RPC] Received Update! {0}", args.Presence);
			Program.PrintOutput(output);
		}

		private static void Client_OnError(object sender, DiscordRPC.Message.ErrorMessage args)
		{
			string output = string.Format("[Discord RPC] Failed Update! {0}", args.Message);
			Program.PrintOutput(output, 1);
		}

		private static void Client_OnConnectionEstablished(object sender, DiscordRPC.Message.ConnectionEstablishedMessage args)
		{
			string output = string.Format("[Discord RPC] Connection Established [Pipe: {0}, Message: {1}]", args.ConnectedPipe, args.Type);
			Program.PrintOutput(output);
		}

		private static void Client_OnConnectionFailed(object sender, DiscordRPC.Message.ConnectionFailedMessage args)
		{
			string output = string.Format("[Discord RPC] Failed to Connect [Pipe: {0}, Message: {1}]", args.FailedPipe, args.Type);
			Program.PrintOutput(output, 1);
		}
		#endregion

		#region Discord Methods

		#region Status Reports
		private enum InitilizationType : int
		{
			AddEventHandlers = 0,
			RemoveEventHandlers = 1,
			SetupTimer = 2,
			AttemptInitilization = 3,
			AttemptDisposal = 4
		}

		private static bool IsClientExistant
		{
			get
			{
				if (client != null) return true;
				else return false;
			}
		}
		private static bool IsInitilized
		{
			get
			{
				if (client != null) return client.IsInitialized;
				else return false;
			}
		}
		private static bool HasAttemptedInitilization { get; set; } = false;
		private static bool AreEventsInitilized { get; set; } = false;
		private static bool HasTimerStarted { get; set; } = false;
        #endregion

        public static void InitDiscord()
		{
			InitializeComponents(InitilizationType.SetupTimer);
			InitializeComponents(InitilizationType.AttemptInitilization);
			InitializeComponents(InitilizationType.AddEventHandlers);
		}
		public static void UpdateDiscord()
		{
			if (MainDataModel.Settings.ShowDiscordRPC)
			{
				InitDiscord();
				SetDiscordRP();
			}
			else
			{
				if (IsClientExistant)
				{
					Program.PrintOutput("[Discord RPC] Turning Discord RPC OFF...");
					DisposeDiscord();
				}
			}

		}
		public static void DisposeDiscord()
		{
			InitializeComponents(InitilizationType.RemoveEventHandlers);
			InitializeComponents(InitilizationType.AttemptDisposal);
		}
		public static void SetDiscordRP()
		{
			if (IsInitilized)
			{
				Program.PrintOutput("[Discord RPC] Updating Discord RPC...");
				client.SetPresence(Presence.GetRichPresence());
				client.Invoke();
			}
		}
		private static void InitializeComponents(InitilizationType type)
		{
			switch (type)
			{
				case InitilizationType.AddEventHandlers:
					AddEventHandlers();
					break;
				case InitilizationType.RemoveEventHandlers:
					RemoveEventHandlers();
					break;
				case InitilizationType.SetupTimer:
					SetupTimer();
					break;
				case InitilizationType.AttemptInitilization:
					AttemptInitilization();
					break;
				case InitilizationType.AttemptDisposal:
					AttemptDisposal();
					break;
			}

			void AddEventHandlers()
			{
				if (!AreEventsInitilized && IsClientExistant)
				{
					Program.PrintOutput("[Discord RPC] Initializing Discord RPC Event Handlers...");
					client.OnReady += Client_OnReady;
					client.OnPresenceUpdate += Client_OnPresenceUpdate;
					client.OnError += Client_OnError;
					client.OnConnectionFailed += Client_OnConnectionFailed;
					client.OnConnectionEstablished += Client_OnConnectionEstablished;
					client.OnClose += Client_OnClose;
					AreEventsInitilized = true;
				}
			}
			void RemoveEventHandlers()
			{
				if (AreEventsInitilized && IsClientExistant)
				{
					Program.PrintOutput("[Discord RPC] Disposing Discord RPC Event Handlers...");
					client.OnReady -= Client_OnReady;
					client.OnPresenceUpdate -= Client_OnPresenceUpdate;
					client.OnError -= Client_OnError;
					client.OnConnectionFailed -= Client_OnConnectionFailed;
					client.OnConnectionEstablished -= Client_OnConnectionEstablished;
					client.OnClose -= Client_OnClose;
					AreEventsInitilized = false;
				}
			}
			void SetupTimer()
			{
				if (!HasTimerStarted)
				{
					timer = new System.Timers.Timer();
					timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
					timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
					timer.Start();
					HasTimerStarted = true;
				}
			}
			void AttemptInitilization()
			{
				var processes = System.Diagnostics.Process.GetProcessesByName("Discord");
				if (processes == null || processes.Count() <= 0)
				{
					if (IsClientExistant) AttemptDisposal();
					return;
				}

				if (!IsInitilized)
				{
					bool wasSuccessful = false;
					if (!HasAttemptedInitilization || !IsClientExistant)
					{
						Program.PrintOutput("[Discord RPC] Starting Discord RPC");
						client = new DiscordRpcClient(APP_ID);
						//client.Logger = new DiscordRPC.Logging.ConsoleLogger { Level = DiscordRPC.Logging.LogLevel.Warning };
						wasSuccessful = client.Initialize();
						HasAttemptedInitilization = true;
					}
					else
					{
						Program.PrintOutput("[Discord RPC] Re-attempting to Start Discord RPC");
						wasSuccessful = client.Initialize();

					}

					if (wasSuccessful) Program.PrintOutput("[Discord RPC] Successfully Started Discord RPC");
					else Program.PrintOutput("[Discord RPC] Failed to Start Discord RPC");
				}


			}
			void AttemptDisposal()
			{
				if (IsClientExistant)
				{
					Program.PrintOutput("[Discord RPC] Closing Discord RPC Support...");
					client.Dispose();
					client = null;
					HasAttemptedInitilization = false;
				}

			}
		}

		#region Rich Presence
		public static class Presence
		{
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

			public static void GetImageTextDetails(ref RichPresence richPresence)
			{
				//richPresence.Assets.LargeImageText
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

				GetImageTextDetails(ref richPresence);

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

		#endregion

		#endregion

	}
}
