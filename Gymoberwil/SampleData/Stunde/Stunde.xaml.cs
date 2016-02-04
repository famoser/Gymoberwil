//      *********    DIESE DATEI DARF NICHT GEÄNDERT WERDEN     *********
//      Diese Datei wurde von einem Designwerkzeug erstellt. Änderungen
//      dieser Datei können Fehler verursachen.
namespace Expression.Blend.SampleData.Stunde
{
	using System; 

// Um den Speicherbedarf der Beispieldaten in der Produktionsanwendung deutlich zu reduzieren, legen Sie
// die Konstante für bedingte Kompilierung DISABLE_SAMPLE_DATA fest, und deaktivieren Sie die Beispieldaten zur Laufzeit.
#if DISABLE_SAMPLE_DATA
	internal class Stunde { }
#else

	public class Stunde : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public Stunde()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/Gymoberwil;component/SampleData/Stunde/Stunde.xaml", System.UriKind.Relative);
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

		private string _Zeit = string.Empty;

		public string Zeit
		{
			get
			{
				return this._Zeit;
			}

			set
			{
				if (this._Zeit != value)
				{
					this._Zeit = value;
					this.OnPropertyChanged("Zeit");
				}
			}
		}

		private string _FachTeil2 = string.Empty;

		public string FachTeil2
		{
			get
			{
				return this._FachTeil2;
			}

			set
			{
				if (this._FachTeil2 != value)
				{
					this._FachTeil2 = value;
					this.OnPropertyChanged("FachTeil2");
				}
			}
		}

		private double _Opacity = 0;

		public double Opacity
		{
			get
			{
				return this._Opacity;
			}

			set
			{
				if (this._Opacity != value)
				{
					this._Opacity = value;
					this.OnPropertyChanged("Opacity");
				}
			}
		}

		private string _FachTeil1 = string.Empty;

		public string FachTeil1
		{
			get
			{
				return this._FachTeil1;
			}

			set
			{
				if (this._FachTeil1 != value)
				{
					this._FachTeil1 = value;
					this.OnPropertyChanged("FachTeil1");
				}
			}
		}

		private string _Zimmernummer = string.Empty;

		public string Zimmernummer
		{
			get
			{
				return this._Zimmernummer;
			}

			set
			{
				if (this._Zimmernummer != value)
				{
					this._Zimmernummer = value;
					this.OnPropertyChanged("Zimmernummer");
				}
			}
		}

		private string _Lehrer = string.Empty;

		public string Lehrer
		{
			get
			{
				return this._Lehrer;
			}

			set
			{
				if (this._Lehrer != value)
				{
					this._Lehrer = value;
					this.OnPropertyChanged("Lehrer");
				}
			}
		}

		private double _OpacityContrary = 0;

		public double OpacityContrary
		{
			get
			{
				return this._OpacityContrary;
			}

			set
			{
				if (this._OpacityContrary != value)
				{
					this._OpacityContrary = value;
					this.OnPropertyChanged("OpacityContrary");
				}
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ 
	}
#endif
}
