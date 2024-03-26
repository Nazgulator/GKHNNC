using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.Models
{
    public class MKDYearOtchet
    {
       public  decimal KapRemont = 0;
        public decimal Arenda = 0;
       public decimal Soderganie = 0;
       public decimal DopTekRem = 0;
        public decimal TekRem = 0;
        public decimal NepredRemont = 0;
        public decimal TEKREM = 0;

        public decimal DopTekRemOld = 0;
        public decimal ArendaOld = 0;
        public decimal NeotlogniOld = 0;
        public decimal TekRemOld = 0;
        public decimal SoderganieOld = 0;

        public decimal ArendaVosnagragdenie = 0;

        public decimal OstatkiDopTekRemSTART = 0;
        public decimal OstatkiTekRemSTART = 0;
        public decimal OstatkiKapRemSTART = 0;
        public decimal OstatkiSoderganieSTART = 0;
        public decimal OstatkiNepredRemSTART = 0;
        public decimal OstatkiArendaSTART = 0;

        public decimal OstatkiDopTekRemEND = 0;
        public decimal OstatkiTekRemEND = 0;
        public decimal OstatkiKapRemEND = 0;
        public decimal OstatkiSoderganieEND = 0;
        public decimal OstatkiNepredRemEND = 0;
        public decimal OstatkiArendaEND = 0;

        public decimal OstatkiArendaNachisleno = 0;
        public decimal OstatkiArendaOplacheno = 0;

        public decimal ORCDopTekRemSTART = 0;
        public decimal ORCTekRemSTART = 0;
        public decimal ORCKapRemontSTART = 0;
        public decimal ORCSoderganieSTART = 0;
        public decimal ORCNepredRemontSTART = 0;

        public decimal ORCDopTekRemCHANGE = 0;
        public decimal ORCTekRemCHANGE = 0;
        public decimal ORCSoderganieCHANGE = 0;
        public decimal ORCKapRemontCHANGE = 0;
        public decimal ORCNepredRemontCHANGE = 0;

        public decimal ORCDopTekRemPAY = 0;
        public decimal ORCTekRemPAY = 0;
        public decimal ORCNepredRemontPAY = 0;
        public decimal ORCSoderganiePAY = 0;
        public decimal ORCKapRemontPAY = 0;
        public string Adres = "";
        public int AdresId = 0;

        public decimal ArendaRaschet;
        public decimal TekRemRaschet;
        public decimal SoderganieRaschet;
        public decimal NepredRaschet;
        public decimal DopTekRemRaschet;

        public decimal TEKREMRaschet;
        public decimal TEKREMNachisleno;
        public decimal TEKREMOplacheno;
        public decimal TEKREMStart;
        public decimal TEKREMEnd;

        public List<MKDCompleteWork> CompletedWorks;
        public List<string> Stati;
        public List<MKDStatya> MKDStatys;
    }
}