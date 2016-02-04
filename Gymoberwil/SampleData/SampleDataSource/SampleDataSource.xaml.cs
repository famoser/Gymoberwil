//      *********    DIESE DATEI DARF NICHT GEÄNDERT WERDEN     *********
//      Diese Datei wurde von einem Designwerkzeug erstellt. Änderungen
//      dieser Datei können Fehler verursachen.
namespace Expression.Blend.SampleData.SampleDataSource
{
	using System; 

// Um den Speicherbedarf der Beispieldaten in der Produktionsanwendung deutlich zu reduzieren, legen Sie
// die Konstante für bedingte Kompilierung DISABLE_SAMPLE_DATA fest, und deaktivieren Sie die Beispieldaten zur Laufzeit.
#if DISABLE_SAMPLE_DATA
	internal class SampleDataSource { }
#else

	public class SampleDataSource : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public SampleDataSource()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Gymoberwil;component/SampleData/SampleDataSource/SampleDataSource.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private ItemCollection _Collection = new ItemCollection();

		public ItemCollection Collection
		{
			get
			{
				return this._Collection;
			}
		}
	}

	public class Item : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		private string _Beschreibung = string.Empty;

		public string Beschreibung
		{
			get
			{
				return this._Beschreibung;
			}

			set
			{
				if (this._Beschreibung != value)
				{
					this._Beschreibung = value;
					this.OnPropertyChanged("Beschreibung");
				}
			}
		}

		private string _Durchschnitt = string.Empty;

		public string Durchschnitt
		{
			get
			{
				return this._Durchschnitt;
			}

			set
			{
				if (this._Durchschnitt != value)
				{
					this._Durchschnitt = value;
					this.OnPropertyChanged("Durchschnitt");
				}
			}
		}

		private string _Datum = string.Empty;

		public string Datum
		{
			get
			{
				return this._Datum;
			}

			set
			{
				if (this._Datum != value)
				{
					this._Datum = value;
					this.OnPropertyChanged("Datum");
				}
			}
		}

		private string _Not = string.Empty;

		public string Not
		{
			get
			{
				return this._Not;
			}

			set
			{
				if (this._Not != value)
				{
					this._Not = value;
					this.OnPropertyChanged("Not");
				}
			}
		}

		private string _Zählweise = string.Empty;

		public string Zählweise
		{
			get
			{
				return this._Zählweise;
			}

			set
			{
				if (this._Zählweise != value)
				{
					this._Zählweise = value;
					this.OnPropertyChanged("Zählweise");
				}
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ 
	}
#endif
}
