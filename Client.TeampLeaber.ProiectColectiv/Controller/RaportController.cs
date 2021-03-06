﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.TeampLeaber.ProiectColectiv.Models;
using Client.TeampLeaber.ProiectColectiv.Views;

namespace Client.TeampLeaber.ProiectColectiv.Controller
{
    public class RaportController
    {
        private MainForm _view;
        private EditInmormantareForm _editView;

        public RaportController(MainForm mainForm)
        {
            _view = mainForm;
            _view.SetRaportController(this);
        }

        internal async void AfiseazaRegistruInmormantari()
        {
            int an = _view.AnPickerTabRapoarte;

            var request = new Networking.Requests.RaportInmormantariRequest(an);
            List<RaportInmormantareModel> inmormantari = await request.Run();
            if (inmormantari.Count() > 0)
            {
                _view.SetRapoarteInmormantareList(inmormantari);
            }
            else
            {
                ErrorHandling.ErrorHandling.Instance.HandleError(Utils.Constants.ErrorMessages.LISTA_VIDA);
            }
        }

        internal void SetEditInmormantareView()
        {
            _editView = new EditInmormantareForm(this);
            _editView.Show();
            UpdateEditView();
        }

        private async void UpdateEditView()
        {
            _editView.SetCnpTextBox(_view.SelectedRaportInmormantare.Decedat.Cnp);
            _editView.SetNameTextBox(_view.SelectedRaportInmormantare.Decedat.Nume);
            _editView.SetPrenumeTextBox(_view.SelectedRaportInmormantare.Decedat.Prenume);
            _editView.SetDataInmormantare(_view.SelectedRaportInmormantare.Date);
            var cimitire = await this.GetCimitire();
            _editView.SetCimitireComboBox(cimitire);
            _editView.SetCimitireComboBoxSelectedItem(_view.SelectedRaportInmormantare.Cimitir);
            var parcele = await this.GetParcele(_view.SelectedRaportInmormantare.Cimitir.Id);
            _editView.SetParceleComboBox(parcele);
            _editView.SetParceleComboBoxSelectedItem(_view.SelectedRaportInmormantare.Parcela);
            var morminte = await this.GetMorminte(_view.SelectedRaportInmormantare.Mormant.Id);
            _editView.SetMorminteComboBox(morminte);
            _editView.SetMorminteComboBoxSelectedItem(_view.SelectedRaportInmormantare.Mormant);
            var religii = await this.GetReligii();
            _editView.SetReligiiComboBox(religii);
            _editView.SetReligiiComboBoxSelectedItem(_view.SelectedRaportInmormantare.Religie);
        }

        internal async Task<List<CimitirModel>> GetCimitire()
        {
            var request = new Networking.Requests.CimitireRequest();
            List<CimitirModel> _cimitire = await request.Run();
            return _cimitire;
        }

        internal async Task<List<ParcelaModel>> GetParcele(int _cimitirId)
        {
            var request = new Networking.Requests.ParceleRequest(_cimitirId);
            List<ParcelaModel> _parcele = await request.Run();
            return _parcele;
        }

        internal async Task<List<MormantModel>> GetMorminte(int _parcelaId)
        {
            var request = new Networking.Requests.MorminteRequest(_parcelaId);
            List<MormantModel> _morminte = await request.Run();
            return _morminte;
        }

        internal async Task<List<ReligieModel>> GetReligii()
        {
            var request = new Networking.Requests.ReligiiRequest();
            List<ReligieModel> _religii = await request.Run();
            return _religii;
        }

        internal async void UpdateParceleComboBoxByCimitir(Models.CimitirModel cimitir)
        {
            var parcele = await this.GetParcele(cimitir.Id);
            _editView.SetParceleComboBox(parcele);
            _editView.SetParceleComboBoxSelectedIndex(0);
        }

        internal async void UpdateMorminteComboBoxByParcela(Models.ParcelaModel parcela)
        {
            var morminte = await this.GetMorminte(parcela.Id);
            _editView.SetMorminteComboBox(morminte);
            _editView.SetMorminteComboBoxSelectedIndex(0);
        }

        internal async void UpdateInmormantare()
        {
            RaportInmormantareModel inmormantare = _view.SelectedRaportInmormantare;
            inmormantare.Cimitir = _editView.GetCimitireComboBoxSelectedItem() as CimitirModel;
            inmormantare.Parcela = _editView.GetParceleComboBoxSelectedItem() as ParcelaModel;
            inmormantare.Mormant = _editView.GetMorminteComboBoxSelectedItem() as MormantModel;
            inmormantare.Religie = _editView.GetReligieComboBoxSelectedItem() as ReligieModel;
            inmormantare.Decedat.Cnp = _editView.GetCnpTextBox();
            inmormantare.Decedat.Nume = _editView.GetNumeTextBox();
            inmormantare.Decedat.Prenume = _editView.GetPrenumeTextBox();
            inmormantare.Date = _editView.GetDataInmormantareDatePicker();

            if (inmormantare.IsValid())
            {
                var request = new Networking.Requests.ModificaInmormantareRequest(inmormantare);
                bool ok = await request.Run();
                if (!ok)
                    return;
                ErrorHandling.ErrorHandling.Instance.HandleError(Utils.Constants.SUCCESS_MESSAGE);
                AfiseazaRegistruInmormantari();
            }

            _editView.Close();
        }
    }
}
