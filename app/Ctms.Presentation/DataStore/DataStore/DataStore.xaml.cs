//      *********    DIESE DATEI DARF NICHT GEÄNDERT WERDEN     *********
//      Diese Datei wurde von einem Designwerkzeug erstellt. Änderungen
//      dieser Datei können Fehler verursachen.
namespace Expression.Blend.DataStore.DataStore
{
	using System;
	using System.Collections.Generic;

	public class DataStoreGlobalStorage
	{
		public static DataStoreGlobalStorage Singleton;
		public bool Loading {get;set;}
		private List<WeakReference> registrar; 

		public DataStoreGlobalStorage()
		{
			this.registrar = new List<WeakReference>();
		}
		
		static DataStoreGlobalStorage()
		{
			Singleton = new DataStoreGlobalStorage();
		}

		public void Register(DataStore dataStore)
		{
			this.registrar.Add(new WeakReference(dataStore));
		}

		public void OnPropertyChanged(string property)
		{
			foreach (WeakReference entry in this.registrar)
			{
				if (!entry.IsAlive)
				{
					continue;
				}
				DataStore dataStore = (DataStore)entry.Target;
				dataStore.FirePropertyChanged(property);
			}
		}
		
		public bool AssignementAllowed
		{
			get
			{
				// Only assign from loading once
				if(this.Loading && this.registrar.Count > 0)
				{
					return false;
				}
				
				return true;
			}
		}

		private string _IsVisible = string.Empty;

		public string IsVisible
		{
			get
			{
				return this._IsVisible;
			}

			set
			{
				if(!this.AssignementAllowed)
				{
					return;
				}
				
				if (this._IsVisible != value)
				{
					this._IsVisible = value;
					this.OnPropertyChanged("IsVisible");
				}
			}
		}
	}

	public class DataStore : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public void FirePropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(propertyName);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public DataStore()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Ctms;component/DataStore/DataStore/DataStore.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					DataStoreGlobalStorage.Singleton.Loading = true;
					System.Windows.Application.LoadComponent(this, resourceUri);
					DataStoreGlobalStorage.Singleton.Loading = false;
					DataStoreGlobalStorage.Singleton.Register(this);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private string _IsVisible = string.Empty;

		public string IsVisible
		{
			get
			{
				return DataStoreGlobalStorage.Singleton.IsVisible;
			}

			set
			{
				DataStoreGlobalStorage.Singleton.IsVisible = value;
			}
		}
	}
}
