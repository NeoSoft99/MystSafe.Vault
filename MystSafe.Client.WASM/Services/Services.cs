// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
// 
//     Copyright (C) 2024-2025 MystSafe, NeoSoft99
// 
//     MystSafe: The Only Privacy-Preserving Password Manager.
//     https://mystsafe.com
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//     See the GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace MystSafe.Client.App;

public class TestingEventArgs : EventArgs
{
    public bool TestingInProgress { get; }
    

    public TestingEventArgs(bool testingInProgress)
    {
        TestingInProgress = testingInProgress;
        
    }
}

public interface INotificationService
	{
    event EventHandler<TestingEventArgs>? OnChanged;
    Task NotifyChanged(bool testingInProgress);
}

public abstract class NotificationService : INotificationService
{
    public event EventHandler<TestingEventArgs>? OnChanged;

    public async Task NotifyChanged(bool testingInProgress)
    {
        await Task.Yield(); // Allow the calling code to continue running
        OnChanged?.Invoke(this, new TestingEventArgs(testingInProgress));
    }

    public virtual async Task NotifyChanged()
    {
        await Task.Yield(); 
        OnChanged?.Invoke(this, new TestingEventArgs(false));
    }
}

// notifies the chats drawer content that updated is needed
public class ChatsDrawerNotification: NotificationService, INotificationService
{
}

// notifies the chats drawer content that updated is needed
public class SecretsDrawerNotification : NotificationService, INotificationService
{
}

public class ChatNotification : NotificationService, INotificationService
{
}

public class SecretNotification : NotificationService, INotificationService
{
}

// Notifies teh main layout about possible need to close the drawer 
public class MainLayoutNotification : NotificationService, INotificationService
{

}

public enum UIModes { Unknown, Chats, Secrets, Account, Settings, InstantShare, License, MasterKey, Stats };

public class UIModeNotification : NotificationService, INotificationService
{
    private UIModes _UIMode = UIModes.Unknown;

    public UIModes UIMode
    {
        get { return _UIMode; }
        set
        {
            if (value != _UIMode)
            {
                _UIMode = value;
                UIModeChanged = true;
            }
        }
    }

    public bool UIModeChanged = false;

    public async Task NotifyChanged(UIModes uiMode)
    {
        UIMode = uiMode;
        if (UIModeChanged)
        {
            UIModeChanged = false;
            await base.NotifyChanged();
        }
    }
}


public class EULAStatus
{
    public bool HasBeenShown = false;
}

public class DrawerState
{
    public bool IsOpen = false;
}

public enum PageTypes {
    CHAT, CREATE_ACCOUNT, NEW_CHAT, MAIN_LAYOUT, DRAWER, MANAGE_ACCOUNT,
    TESTING, SECRET, NEW_SECRET, LICENSE, NEW_FOLDER, FOLDER,
    INSTANT_SHARE, MASTER_KEY, LOCKED, STATS
};

public static class PageUrls
{
    public const string DEFAULT_PAGE = SECRET;

    public const string MANAGE_ACCOUNT = @"/manageaccount";
    public const string CREATE_ACCOUNT = @"/createaccount";
    public const string NEW_CHAT = "/newchat";
    public const string NEW_SECRET = "/newsecret";
    public const string SECRET = "/secret";
    public const string CHAT = "/chat";
    public const string LOCKED = "/locked";
    public const string CHECKOUT = "https://checkout.mystsafe.com";
    public const string TESTING = "/testing";
    public const string MASTER_KEY = "/masterkey";
    public const string LICENSE = "/license";
    public const string WEBSITE = "https://mystsafe.com";
    public const string FOLDER = "/folder";
    public const string NEW_FOLDER = "/newfolder";
    public const string INSTANT_SHARE = "/instantshare";
    public const string STATS = "/stats";
}



