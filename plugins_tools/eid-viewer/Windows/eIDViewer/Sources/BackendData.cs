﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Resources;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;

namespace eIDViewer
{


    public class eIDViewerBackendData : INotifyPropertyChanged
    {
        public interface ICert
        {
            string CertLabel { get; }
            ObservableCollection<ICert> Certs { get; }
            bool IsExpanded { get; }
            bool IsSelected { get; set; }
            System.Windows.Visibility CertVisibility { get; set; }
        }

        public class CCert: ICert, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public CCert()
            {
                Certs = new ObservableCollection<ICert>();
                IsExpanded = true;
                CertVisibility = System.Windows.Visibility.Hidden;
            }
            public void CertNotifyPropertyChanged(String propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public string CertLabel { get; set; }
            public ObservableCollection<ICert> Certs { get; set; }
            public bool IsExpanded { get; set; }
            public bool IsSelected { get; set; }

            private System.Windows.Visibility _CertVisibility;
            public System.Windows.Visibility CertVisibility
            {
                get { return _CertVisibility; }
                set
                {
                    _CertVisibility = value;
                    this.CertNotifyPropertyChanged("CertVisibility");
                }
            }
        }

        public eIDViewerBackendData()
        {
            _certsList = new List<ICert>();
            cert_collection = new X509Certificate2Collection();
            rootCA = new CCert { CertLabel = "rootCA" };
            RNCert = new CCert { CertLabel = "RN cert" };
            intermediateCA = new CCert { CertLabel = "citizen CA" };
            authCert = new CCert { CertLabel = "Authentication" };
            signCert = new CCert { CertLabel = "Signature" };

            certsList.Add(rootCA);

            certsList[0].Certs.Add(RNCert);
            certsList[0].Certs.Add(intermediateCA);

            certsList[0].Certs[1].Certs.Add(authCert);
            certsList[0].Certs[1].Certs.Add(signCert);
        }

        private CCert rootCA;
        private CCert intermediateCA;
        private CCert RNCert;
        private CCert authCert;
        private CCert signCert;

        private X509Certificate2 authentication_cert;
        private X509Certificate2 signature_cert;
        private X509Certificate2 rootCA_cert;
        private X509Certificate2 intermediateCA_cert;
        private X509Certificate2 RN_cert;

        public event PropertyChangedEventHandler PropertyChanged;
        private X509Certificate2Collection cert_collection;

        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Boolean _eid_data_ready;
        public Boolean eid_data_ready
        {
            get { return _eid_data_ready; }
            set
            {
                _eid_data_ready = value;
                this.NotifyPropertyChanged("eid_data_ready");
            }
        }

        private string _logtext;
        public string logText
        {
            get { return _logtext; }
            set
            {
                _logtext = value;
                NotifyPropertyChanged("logText");
            }
        }

        private string _type_kaart;
        public string type_kaart
        {
            get { return _type_kaart; }
            set
            {
                _type_kaart = value;
                NotifyPropertyChanged("type_kaart");
            }
        }       

        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            try
            {
                BitmapImage image = new BitmapImage();
                MemoryStream mem = new MemoryStream(imageData);

                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (Exception e)
            {
                this.logText+= "An error occurred displaying the image \n";
                Console.WriteLine("An error occurred: '{0}'", e);
                return null;
            }
        }


        private string _firstName;
        public string firstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                _firstNames = value + _first_letter_of_third_given_name;
                this.NotifyPropertyChanged("firstNames");
            }
        }

        private string _surName;
        public string surName
        {
            get { return _surName; }
            set
            {
                _surName = value;
                this.NotifyPropertyChanged("surName");
            }
        }

        private string _first_letter_of_third_given_name;
        public string first_letter_of_third_given_name
        {
            get { return _first_letter_of_third_given_name; }
            set
            {
                _first_letter_of_third_given_name = value;
                _firstNames = _firstName + value;
                this.NotifyPropertyChanged("firstNames");
            }
        }

        private string _firstNames;
        public string firstNames
        {
            get { return _firstNames; }
            set
            {
                _firstNames = value;
                this.NotifyPropertyChanged("firstNames");
            }
        }

        private string _date_of_birth;
        public string date_of_birth
        {
            get { return _date_of_birth; }
            set
            {
                _date_of_birth = value;
                this.NotifyPropertyChanged("date_of_birth");
            }
        }

        private string _location_of_birth;
        public string location_of_birth
        {
            get { return _location_of_birth; }
            set
            {
                _location_of_birth = value;
                this.NotifyPropertyChanged("location_of_birth");
            }
        }

        private string _gender;
        public string gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                this.NotifyPropertyChanged("gender");
            }
        }

        private string _national_number;
        public string national_number
        {
            get { return _national_number; }
            set
            {
                _national_number = value;
                this.NotifyPropertyChanged("national_number");
            }
        }

        private string _nationality;
        public string nationality
        {
            get { return _nationality; }
            set
            {
                _nationality = value;
                this.NotifyPropertyChanged("nationality");
            }
        }

        private string _nobility;
        public string nobility
        {
            get { return _nobility; }
            set
            {
                _nobility = value;
                this.NotifyPropertyChanged("nobility");
            }
        }

        private string _special_status;
        public string special_status
        {
            get { return _special_status; }
            set
            {
                _special_status = value;
                this.NotifyPropertyChanged("special_status");
            }
        }

        private string _address_street_and_number;
        public string address_street_and_number
        {
            get { return _address_street_and_number; }
            set
            {
                _address_street_and_number = value;
                this.NotifyPropertyChanged("address_street_and_number");
            }
        }
        private string _address_zip;
        public string address_zip
        {
            get { return _address_zip; }
            set
            {
                _address_zip = value;
                this.NotifyPropertyChanged("address_zip");
            }
        }
        private string _address_municipality;
        public string address_municipality
        {
            get { return _address_municipality; }
            set
            {
                _address_municipality = value;
                this.NotifyPropertyChanged("address_municipality");
            }
        }

        private string _card_number;
        public string card_number
        {
            get { return _card_number; }
            set
            {
                _card_number = value;
                this.NotifyPropertyChanged("card_number");
            }
        }

        private string _issuing_municipality;
        public string issuing_municipality
        {
            get { return _issuing_municipality; }
            set
            {
                _issuing_municipality = value;
                this.NotifyPropertyChanged("issuing_municipality");
            }
        }

        private string _chip_number;
        public string chip_number
        {
            get { return _chip_number; }
            set
            {
                _chip_number = value;
                this.NotifyPropertyChanged("chip_number");
            }
        }

        private string _validity_begin_date;
        public string validity_begin_date
        {
            get { return _validity_begin_date; }
            set
            {
                _validity_begin_date = value;
                this.NotifyPropertyChanged("validity_begin_date");
            }
        }

        private string _validity_end_date;
        public string validity_end_date
        {
            get { return _validity_end_date; }
            set
            {
                _validity_end_date = value;
                this.NotifyPropertyChanged("validity_end_date");
            }
        }

        private string _document_type;
        private string _document_type_value;
        public string document_type
        {
            get { return _document_type; }
            set
            {
                _document_type_value = value;
                if (String.Equals(value, "1", StringComparison.Ordinal))
                {
                    ResourceManager rm = new ResourceManager("IdentityTabStringResources",
                                         typeof(eIDViewerBackendData).Assembly);
                    _document_type = rm.GetString("type_BELGIAN_CITIZEN", null);

                }
                this.NotifyPropertyChanged("document_type");
            }
        }

        public void Refresh()
        {
            document_type = _document_type_value;
        }

        private BitmapImage _photo;
        public BitmapImage photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                this.NotifyPropertyChanged("photo");
            }
        }

        private BitmapImage _cardreader_icon;
        public BitmapImage cardreader_icon
        {
            get { return _cardreader_icon; }
            set
            {
                _cardreader_icon = value;
                this.NotifyPropertyChanged("cardreader_icon");
            }
        }

        private List<ICert> _certsList;
        public List<ICert> certsList
        {
            get { return _certsList; }
            set
            {
                _certsList = value;
                this.NotifyPropertyChanged("certsList");
            }
        }



        public void StoreStringData(string label, string data)
        {
            progress_info = "reading data";
            progress += 1;
            if (String.Equals(label, "firstnames", StringComparison.Ordinal))
            { firstName = data; }
            else if (String.Equals(label, "surname", StringComparison.Ordinal))
            { surName = data; }
            else if (String.Equals(label, "first_letter_of_third_given_name", StringComparison.Ordinal))
            { first_letter_of_third_given_name = data; }
            else if (String.Equals(label, "date_of_birth", StringComparison.Ordinal))
            { date_of_birth = data; }
            else if (String.Equals(label, "location_of_birth", StringComparison.Ordinal))
            { location_of_birth = data; }
            else if (String.Equals(label, "gender", StringComparison.Ordinal))
            { gender = data; }
            else if (String.Equals(label, "national_number", StringComparison.Ordinal))
            { national_number = data; }
            else if (String.Equals(label, "nationality", StringComparison.Ordinal))
            { nationality = data; }
            else if (String.Equals(label, "nobility", StringComparison.Ordinal))
            { nobility = data; }
            else if (String.Equals(label, "special_status", StringComparison.Ordinal))
            { special_status = data; }
            else if (String.Equals(label, "address_street_and_number", StringComparison.Ordinal))
            { address_street_and_number = data; }
            else if (String.Equals(label, "address_zip", StringComparison.Ordinal))
            { address_zip = data; }
            else if (String.Equals(label, "address_municipality", StringComparison.Ordinal))
            { address_municipality = data; }
            else if (String.Equals(label, "card_number", StringComparison.Ordinal))
            { card_number = data; }
            else if (String.Equals(label, "issuing_municipality", StringComparison.Ordinal))
            { issuing_municipality = data; }
            else if (String.Equals(label, "chip_number", StringComparison.Ordinal))
            { chip_number = data; }
            else if (String.Equals(label, "validity_begin_date", StringComparison.Ordinal))
            { validity_begin_date = data; }
            else if (String.Equals(label, "validity_end_date", StringComparison.Ordinal))
            { validity_end_date = data; }
            else if (String.Equals(label, "document_type", StringComparison.Ordinal))
            { _document_type = data; }
        }

        public void StoreBinData(string label, byte[] data, int datalen)
        {
            try
            {
                progress += 6;
                if (String.Equals(label, "PHOTO_FILE", StringComparison.Ordinal))
                {
                    photo = LoadImage(data);
                    progress_info = "reading photo";
                }
                else if (String.Equals(label, "chip_number", StringComparison.Ordinal))
                { //chip_number = BitConverter.ToString(data);
                    progress_info = "reading chip_number";
                    chip_number = String.Concat(Array.ConvertAll(data, x => x.ToString("X2")));
                }
                else if (String.Equals(label, "Authentication", StringComparison.Ordinal))
                {
                    progress_info = "reading authentication certificate";
                    authentication_cert = new X509Certificate2(data);
                    cert_collection.Add(authentication_cert);
                    authCert.CertVisibility = Visibility.Visible;
                }
                else if (String.Equals(label, "Signature", StringComparison.Ordinal))
                {
                    progress_info = "reading signature certificate";
                    signature_cert = new X509Certificate2(data);
                    cert_collection.Add(signature_cert);
                    signCert.CertVisibility = Visibility.Visible;
                    signCert.CertNotifyPropertyChanged("CertVisibility");
                }
                else if (String.Equals(label, "Root", StringComparison.Ordinal))
                {
                    progress_info = "reading root certificate";
                    rootCA_cert = new X509Certificate2(data);
                    cert_collection.Add(rootCA_cert);
                    rootCA.CertVisibility = Visibility.Visible;
                    rootCA.CertLabel="Root_gevodnen";
                }
                else if (String.Equals(label, "CA", StringComparison.Ordinal))
                {
                    progress_info = "reading intermediate certificate";
                    intermediateCA_cert = new X509Certificate2(data);
                    cert_collection.Add(intermediateCA_cert);
                    intermediateCA.CertVisibility = Visibility.Visible;
                }
                else if (String.Equals(label, "CERT_RN_FILE", StringComparison.Ordinal))
                {
                    progress_info = "reading RN certificate";
                    RN_cert = new X509Certificate2(data);
                    cert_collection.Add(RN_cert);
                    RNCert.CertVisibility = Visibility.Visible;
                }

            }
            catch ( Exception e)
            {
                this.logText += "An error occurred storing binary data of " + label + "\n";
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        public void ResetDataValues()
        {
            text_color = "Gray";
            firstName = "-";
            firstNames = "-";
            surName = "-";
            first_letter_of_third_given_name = "-";
            date_of_birth = "-";
            location_of_birth = "-";
            gender = "-";
            national_number = "-";
            nationality = "-";
            nobility = "-";
            special_status = "-";
            address_street_and_number = "-";
            address_zip = "-";
            address_municipality = "-";
            card_number = "-";
            issuing_municipality = "-";
            chip_number = "-";
            validity_begin_date = "-";
            validity_end_date = "-";
            eid_card_present = false;
            progress = 0;
            progress_bar_visible = "Hidden";
            progress_info = "";
            pinop_ready = false;

            cert_collection = new X509Certificate2Collection();
        }

        private String _text_color;
        public String text_color
        {
            get { return _text_color; }
            set
            {
                _text_color = value;
                this.NotifyPropertyChanged("text_color");
            }
        }

        private Boolean _eid_card_present;
        public Boolean eid_card_present
        {
            get { return _eid_card_present; }
            set
            {
                _eid_card_present = value;
                this.NotifyPropertyChanged("eid_card_present");
            }
        }

        private int _progress;
        public int progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                this.NotifyPropertyChanged("progress");
                //if (progress >= 100)
                //{
                //    progress_bar_visible = "Hidden";
                //}
            }
        }
        private String _progress_bar_visible = "Hidden";
        public String progress_bar_visible
        {
            get { return _progress_bar_visible; }
            set
            {
                _progress_bar_visible = value;
                this.NotifyPropertyChanged("progress_bar_visible");
            }
        }
        private String _progress_info = "";
        public String progress_info
        {
            get { return _progress_info; }
            set
            {
                _progress_info = value;
                this.NotifyPropertyChanged("progress_info");
            }
        }

        private eid_vwr_loglevel _log_level = eid_vwr_loglevel.EID_VWR_LOG_DETAIL;
        public eid_vwr_loglevel log_level
        {
            get { return _log_level; }
            set
            {
                _log_level = value;
                this.NotifyPropertyChanged("log_level");
            }
        }

        private Boolean _pinop_ready = false;
        public Boolean pinop_ready
        {
            get { return _pinop_ready; }
            set
            {
                _pinop_ready = value;
                this.NotifyPropertyChanged("pinop_ready");
            }
        }

    }
}
