using System;

namespace SysValley.Danfse
{
    /// <summary>
    /// Recursos graficos embutidos no DANFSe. A logomarca foi extraida do DANFSe v2.0
    /// gerado pelo ambiente nacional (PNG RGBA 540x107, com transparencia).
    /// </summary>
    internal static class DanfseRecursos
    {
        private static byte[] _logoNfse;

        /// <summary>Logomarca "NFS-e / Nota Fiscal de Servico eletronica" (PNG).</summary>
        internal static byte[] LogoNfse
        {
            get { return _logoNfse ?? (_logoNfse = Convert.FromBase64String(LogoNfseBase64)); }
        }

        private const string LogoNfseBase64 =
            "iVBORw0KGgoAAAANSUhEUgAAAhwAAABrCAYAAAA8RlssAAAnMklEQVR42u19f2wb153nR5TsOHasUElzzo+6odvrlqn3EIrAoXGBnjnAXWK0wJ" +
            "rCZQ3sLReiUu8fCwGVJhGIw2EJSSvcAicoGenumAO2TkihLIo1Aog+IAs3u1iOs3+4xd7RDHDeaAv4zFSJkzbnmLYTJ7Et6f7gd+rn8Qw5M5wZ" +
            "DsnvByAsS/Pjve97fO/zvj8BBoPBYDAYDAaDwWAwGIxuxwCLgNGriCupYQAHAeynj/Z/M1wF8E/083kAGxW5eJ4lyWAwGH1KOKLSUsIX4QxtAU" +
            "NbTa+5+ekT+l/VL/zDn1R5anWEYBwEcIg+GtFwA+fpcxbA2Ypc3GBpMxgMhkuEI66kIgBmASQARJw8/Pvha/iXez7H1+//HE89cBV7Bm+5s7HM" +
            "HvdMIIMD25ub2wM3ANwGcAvAdrPrt7fv27m9PTi4vX3ffVvbO+8DoF74hz+R2tg0EwCmSO7hLp9fyxW5KPtAMo4BeM5FgmGFgJwFcJI1IAwGg9" +
            "EG4YgrqTkiG67i++Fr+A+PXUJk96dtPSe5/Mf49ZX7XRfGjsGtz29thq45vX9ra/f9m1t7K04JR1xJKQCme2yOTVTkYsEDonEMwIs+koxm5OME" +
            "gNMVuXiNlxQGg8EwRshk05v14mV/Ux9G6t0o1E/2tfWcx0c+c71tQ6HtzXbIBgCEQjc+Hxq89rcON9B0D5INoKGtcZVoxJXULwAoASAbQMN0ow" +
            "D4ZVxJvUR+IwwGg8FoRjhIne/5pvfnF/e3RTr23nfT9TYNDGxvukJcwlcGHd4626NzLOYS0TgUV1JvBYho6DGMhsbll3EldYSXFgaDwWiu4Rj3" +
            "68V/fnE/3r0+4ujebz12JajyHNjxVfuEI66kwnDoJ4PucOZMtHn/SwDeaBFhEiTi8RqZfBgMBoNhQjhifr78P144gM82d9i+73vRWkClub0z9I" +
            "3fdEwL0IthraTVeLELm6+QQyuDwWAwOk04Lm+G8NeXnrB93zf3fYgH778VOGEOPXH1Pp5SrkafdItWwwys5WAwGAwzp1G/8fpvv4KPb+6yfd/Y" +
            "aC1oshwY2nd1F08pV8jG/h4gG+iB9jMYDEbvEA4A+JkDLcexZ/5PsAS598v7Q49f4cyt7mQHfQ0NX4huB0esMBgMRpAIx8nLI7YdSB998DL+ff" +
            "y9oMhxYOc3/t8DPJ1cwes9pBlgDQeDwWAEiXAAwE8+fMz2PT9MVALhyxEa/nJP6Gsfs3YDbWs3XkIjLTmDwWAwmHB4g7ev73ak5fgzqbOmlYGB" +
            "7aGdT2/s4ankipPoiywJBoPBYMLhOV5934Evx3f+N/7dU5c65yj67Y/CoZFPeSY1R83CNfMsJgaDwWDC4QvO3djlKANpNqni4ONX/U/y9fXLD+" +
            "34xm8GeRo1Rb0iF2utsoiCTSkMBoPR0xgKWoPyHzyKxEP2kmc9sOsG/sf4m/iz1R/g/KUH/SMbv78xxFOoJUoWrnnJh3ZcA3Aad0rNoyIXzwph" +
            "uF/FHUfP/fQvkyAGg8HoVcJx4eYOqJ/sc0w6Mn/9LH7xf7/iqc/Gjm9+/OBQ9BKTjdaoo4WpxAftxlkAJypy8bTZBRW5uAFgQ7heDNH9LoDnAB" +
            "wBh7kyGAxG7xAOoKHl+NcPfoI9g7dsk45Xx0t4/cwh/Pe/dz8iMXT/7d07YhsPDD5ytRsiUgoAOh03XGplToF32TivAXi5IhdPOH0AlZs/DeB0" +
            "XEnNEuk4ZoMgneclhsFgMAJMOC7c3IE3P34Uxx7dcHT/C4fP4vuxX+EvSt9zR9sR2t6588lP9gw9dWknhja7ZWxXK3JR7YJ2HvGIbDxfkYuubf" +
            "hEPk4COEkRNcctkKVrvMQwGAxGAJ1GRfz0o0ccFXaDEDL76ngJr73wd04ThA2Edt2+f+fv/fbh3f/23ZGhf7XRTWQDXRIKewjemClecJNsGJCP" +
            "8xW5KAN4hkgIWMPBYDAY6D4NB9Ao7NaOlkPD6JM1jD5Zw9Rzu3Huvf04V3sU6x+N4N0PH8TVz+8iNKHQ3i93h/bcHAw99OnO0MOfDXG4q+f4rg" +
            "fPPKk5g3oN8v2Q40rqBBq+KnpTy2keYgaDwQg44QAaWo4fPPKRbV8OmPh3fO9b/4zvfeufDf/+7vWRoT/91Tf28pTwFV44i77idydIm/I8aWyO" +
            "A3gQwFm/iA+DwWAw4UD7Wo7XNr6GH0Uu8Ej1Jtz27D1PWoeOgAgGkwwGg8GAPz4cdbhc2M1J+XoG+rGa6s9ZpAwGg9E/hKMKa6ms4WX5ega6oX" +
            "aK29hgyTIYDEb/EI4IgBWwloMBX7UbTDgYDAajDwlHwe2HKrWv82gxWmE/i4DBYDD6KA9HRS7W3SYdTsrXM5hwMBgMBqP3E3+tuP3An3z4GI8Y" +
            "oxmeYxEwGAxGnxGOilysAlDBWg6GfzhIlV8ZDAaD0WepzVnLwfAbcywCBoPB6BPCEVdSMQCoyMUSXA6Rffv6bqif7OOR6354VWfkSFxJHWPxMh" +
            "gMRn9oOMLCz/PwoHy927jfhfTpDNitvuoVFCYdDAaD0WfVYitysQCXtRwXbu5wXcsR2c2F2tA7Wg4mHQwGg9Gn5elX0QVaDobv8DpRlxJXUvNx" +
            "JTXMomYwGIz+IBzLcLnGyoWbO3DyIw5I6HL4UejsOIC3qJIrg8FgMHqZcFAisBUvytd/trmDR7F78XMfE4K9EVdSr3HYLIPBYPS2hgPwQMtxeT" +
            "OENz9m0wq613F0A976cehxBMAv4kpKYeLBYDAYPUo4WMvBMMHJDrzzmEA82NTCYDAYPabhAFjLwTAmHNc69O5jaJha3uCIFgaDweghwuGVluO/" +
            "frCPy9ejq/NxnOhwMw6hEdHyi7iSeomjWhgMBsN9DHXgncsApnB3crC28bNLT+BHkQs8ot2JH6MRTTIcgGqzLwI4HldSJwGcID+TQCIqLcV036" +
            "P6enmmytOJwWAw4SAtR1xJrQCYdfO5Jy+P4I8e34VHdn7Bo4ru03LEldTL8CArrUMMEwHSiMcrnSQeRCwSAJ4GEKOP2bXajyqAKoB3AJTWyzN1" +
            "nmkMBqPfNByoyMW5uJIaBxBhLQeD5sSJuJI6goZ5I0g4BuBYXEmdBjDnF/GISksRNDSBSYffkwR9ACAflZZUAKvr5ZkCzzZGtyOTzW1rPy8uTA" +
            "54/K6y8F2SFhcmVR6BLiIcuFNjJe+2luMP/sUDnKa8gXJcSXXq3SqAMfLZsQMZwFsBMK3AJKT2iNcaj6i0lEBD+5dw+dEJAImotKQAWFkvz8z5" +
            "vEGUhf+uLC5MlizeFwOg0H9XFxcmC322sZZt3nKXjDLZ3DSAw/T7Ei+LDPRRlIqnNVYA4K/e/xqPaueRADDtMC/HCwHvmxZS66pzaVRaikSlpT" +
            "UAZQ/IBnSFFWej0tJFIjd+zgntk89kc2Eb7dXui/Tpd8nOJyKQjTSRtSSAtUw214/yYzDhuOtEC7fL1797fYRHtvMYd0hEz3oxLzzAiwB+SWag" +
            "dslGGsA52hj8QgRAmTQefiMsaC2CqFWIZbK56Uw2NxewpqkWPjWTqt1w21GfwUAXmVRQkYuluJJS3T7R/eTDx/CXe6/w6HYWkbiSCjswq6AiF0" +
            "+SOUgJeB+HAbxG/h0yhfjaJRt5AOkO9mGanFLHfHYsTWeyudWg2cMz2VyCtEzaBh8YLC5MSjZvKQA4Suvr8uLCJEcwMfpawwF4EJnAWo7AINYG" +
            "GT3ZJZoOoOHf8Us7WUuj0lI4Ki2d6zDZgKC2L0elJT9OwAXhZzumFYZ9glJfXJiUFhcmBxYXJmWWCKPvCUdFLqpenCReff8JHl10feTKSQDPon" +
            "OZSGFT22EpYylt7OV2CBl9Z0pE2LVPoY3vUgzAmg9yek8gHRE48PVhMBhgkwra8+U45+YDz93YBfWTfUg89Bse5e4mHefjSuo7AF5H8EJmjaDE" +
            "ldTBilxslmdmzQHZqNNGfWq9PKNaIDVJNNTpaTuajqi0pKyXZ7w8DT9I5Ehr12wmmyt5oe4n8wj8NNto7wRQXVyYrHdhVExbMiPH1IidZ5CWK+" +
            "b3WJlERIXdGDtRDt06F3qWcFTkYjWupApuq5fzHzzKhKN30p8/H1dSxwG8FNCwWRHH40pquCIXZQMioNj0WdLKASzb8bFYL8+UAJSi0tI8hdmm" +
            "bfh0WCI1DhFbXJisZbK5ZUG7kQcw6sKGEaZnHhUJXSabAyVBO0W+DPUWuRZ+R8DEfA8wyMNAG/Q4Gg6/Yd0za2iEo84FwR/FzAeEolmmDGRWA7" +
            "BqJjPd/Uf1Ts/iM/QyaHFPFcC8H2G8RAy072RY+H2VwrcLNp83R/MhYjAX5vstrBsB9OGAV74cF27ugPrJPt6xeyg5GIDvoDMVZu3imN68QloH" +
            "OyaEKoDR9fLMnFOHzvXyTG29PDMBYMxG4cS8T993rT2xdiNC6HR6kciVkfYoRn87R9e6sVmlaUNPm0SAREiDcy6oviqZbC5P4x0zaf+4GdnIZH" +
            "ORTDZ3ju5PNomGetrknclm5j26zsu+p2nOJA3GL0Y+RnmrZJdkMWsSvh2x8zwmHN5vJjXc7VDmmpaD0VvaDtIcPA/gLIJvXjkk+G3YWWxKAKT1" +
            "8kzNjYaQxkOySDoiUWlpzmuHRtztFDzlNE8EEYhzwqZRAjBGzpIDRLaqYjiwwbtkko+sI3yS7lPV+dHUqT6UBGCE3jcCYEIkVG6XcnBxw00LfR" +
            "VlJlG/VptoBs4JRKVGshvVPWPe4BlnhOsl4foDuoNnOpPNJT3qe1L3fSxobdGNX7qVRpLIpOiTtQzggMn8Swcw3Br95sMhnnqSbsaLX7i5A29+" +
            "/Dh+8Mgl3q17i3icJTPLIZo3BwPa1NfjSuo7N/4nZm3M6+p6eWbM7Yasl2eqUWlpTFCzN8NUVFpa9jJUdnFhspDJ5sYFdXaeNim7ZhSxP/Liwu" +
            "Sy7j0lAKVMNrcmrC93vUvzISGVvoZ6M58CMg0d0GsA6P8FUstrvmnTmWxuvh1bvoWNqmZTZT8l/Dy2uDBZE/qgtnBAXhPmc2FxYXLCQD5mzygZ" +
            "tZPeP5fJ5upCSPwsEUg3yYae/E+I7RHGr0Tj14oIKwLZmND3bXFhspTJ5lThWbOZbK4gyps1HJ3Tcrhevv6vPngUn23u4F26R4lHRS4+C+CHAd" +
            "V4DOOLHas2TCl1u5uuTdKhWjRfhn1KRCbrfCbSNu9PCxufqicbOohah4Tg4Nmupsbsb1XdZtmuKWe2xWfcadi6nc2PxigmOEROuCUz+vuyTjvk" +
            "NqZFbZgZSaN2TljwAUkL86/Zs8TvXRJgk0oQsGzD1mwJlzdDePNjNq10AFUficfpilx8nkwtp4MkhK3B7WeHHvrc6uUTXifgojoqNZsnYK+0HF" +
            "XdQqzY9HcYt+oHRov+SrvZcG3iHV2+kyChpqu54kQzMu/12uEGMdThqPDzSos5o7b4rqStPktHPo+CTSqBOLF6Ur7+px89gh888hH2DN5iGuDT" +
            "YuYky6hLppazcSW1H43048c6KYSBbYRCmwO7dvz++7j99jdbXa6SrwUCUjwxFpWWYuvlmaoPhwzNs19Lez7h4JSuwlr+klm3T8900k2i4SAZoW" +
            "eHXSZnbldFXRFMF0ommztMkRmqDZmXXHD21eQW9omU2Z0ztSZmlcPCz3Ub5CgGsIYjSFqOGljL0c1Y7TBx3SDn0qcAvIIOJQ/b3ArtAhAa2HUT" +
            "O5+46nukVovTVt1iFlL44EA6oXMWTNjIeWE5Dblug4m5QTTIN+Qibd5pfYglgpuJdBl3O+on0XCovWhm2nIic7PnUFSHFtmR9GOuudV+3B19oq" +
            "Fs4YN+rmsTCqhdvu7F4vvTjx7Bxzd3mf59dPcXTBPgmjp0OUBRLS9X5OJTaPgLbPj5/sHboT3az0OP11s5iqp+tYvMNiWb6mcvNz9V1x6lC5Jk" +
            "xXRF92o07ydwJ6qlEHDSoYVM1wxCOF0LITYJJ44J68U8tcMoGijIiPByj+41qYjl6+NKatbNAb28GcLPLj2BH0UuGP597+BWL42t3KkvLaWrD2" +
            "qq9JOUH+NFAPvhsTlle3vgjrfyyKcIDX+JrWv3wUS97TfOWEgIlvCxPROCdiCWyebSFMnidoQCBAfddp5TFk6qspHDqgf+B16QDi2KJ4GGaSst" +
            "aIDKRpE4bWoY8oL8x4zMGhSp0g2oa3PAA5MXEw74HybraqKUk5dH8EeP78IjO3tem1EN6sbfL8Rjcyu0S69CHHq8jpvX9rVyKIPFAnAROl0f1l" +
            "IyAzhjww+kavE9fvhxYHFhsp7J5mThOz/bTEOwuDCpCmQk4cB23k6f0gLZWG4RHYMuKfamAlAz2dyKQKbC1NdlI5lnsrmwTTIypQtHVf3uo4M5" +
            "0+o7lNDMa/0Y6ooe8OH4nZbDi8JuP7vEhd0YDeJRkYvPoOHj4T62QjvvYfgjn5mZU+o2yYaCO34Dmv17GsBaVFo6RyXn0Sovh8XXhX3cEMTvfM" +
            "RCmGxVp6qHjaiWU2009bDF5zzYhcSjqgtXPtqEqCYdVCa24nAa9ik6J2FBk5WwSNrTvKp2MeHwypHu5OWRpr4cjL4jHi+jUZX2vJvPHdzCvclf" +
            "9n6OgaEto8gJO2Qj3yKvR4zKzcdcLF8Pn00rELQcaBFpAaEQXLiFz0XahlYpbPFvza7r1nwLNYsO4XbDmMMm5i19xE/MJ4f2WQs5O5rhlC5bbl" +
            "86g/YM4fCqfP2PN77Go8+4qyotGjk8XKvTcpf/hkhEhr+EQcl2q2QjbfEkFbZYbr4WwBN2TThoRCxoRMS05WtGiz6RDTFKYNlI/a2PYmniNFlt" +
            "lc+D6mZEEPDKsGht+tBrcArCvAmbpIp3pBGgsVvzuOsF3J0ALt/EuXXWhrOzJVn0MykZ6pJ2zrt9yvqb+jDGro/gqb1XeLdliFVp5biSAtrM37" +
            "G1PTBoxuZDez8HPrnfaYI0O/lpIlFpaRvd6U8wl8nmjlo86U4IPgcJNAq0lWijDKNhEkjrUmvLLTZF7b3lTDZXAHCVUp0vC5oV7fSbpGqzK7SR" +
            "xWjDjuieFSSyoW2ONdowz1DbI0SgEoJTZMHA12ZMkHkMwEWS0xmBjCRI9qeEarEruOOjo2SyuacFjcNRwTfGM7lRSnrRVyhNxHKV3htBw2SWpv" +
            "6XWpD8CSH3ipksIvQ5Su+YAGs4Aq3lKLj93J98+Bjvsgyj+Sa3q+kIbTch8zs2YZBu3JLzZp+F4ck2fA4kCL4fRAbKdFoWN4v5xYXJMRsmHa3k" +
            "/axI9kg7MqEzO63ROxVqw7LumiD5cyR0stLanhf+VqOCZnULMtc0Fnkh54RWufewTiNV0N2jXT8tyH/VS38Oaoe+wJ4iyCAtlBlYtZBHRjLoly" +
            "iLvCCPBNik0n++HG9f3413r4/wDssw0yR4k7NjyHH4dTerYlX6VG1GTcwL99aakY7FhUmJNpGC8J4a3TuPRgXPOYsEZlSXHK2uN+3SpiXpTAxV" +
            "ofKoTM9S2zALq23cXzeTOzlsahVhVaGfWv2XCar8WrUgc0knc+05mtznDXJ/TOjkq1JbRgXHYe0Tdlku4viN6mSgyUym+VLVybFuRjqoXwcMZK" +
            "q1tyRUJe5L3BU3HFdSbqlfJS9CMuNKKu+2J/C/2XsDf/l7/wQA+E+/+jbevr67bWJUkYtzNvuVgLUKnh0fg34Chc06S0B1e3APboeMT7RXHsCN" +
            "f3xS+19tvTxzwKKGw4t5YmlOU/0VBoPB6AsNh2dajv919Ss8Exgwydex4fF3KxzEYngMBoPR14SDyte7Tjr+2685LwfDFF6XvA/bTEeudkAGJZ" +
            "4GDAYDfRKlAl1htyk37dkXbu6A+sk+ng0MI7yP7o3YUtHwlG8Hqh9ZRhkMBhMO9Ev5+vwHj2J4aJNnBEMPR8nAtga2N83Uh5u3Q+0UXVOj0tKE" +
            "hZT/VQBjdjOYMhgMBtikAr2Ww9WF9MLNHTh3g7OPMu7BVx19sQa2Tdnr9vVd7VZ6LeDekEQIkQnzACQmGwwGA6zhgBtaDtntwm4MBjpQCyMqLS" +
            "Xslqan61Uq4Baxm9ODwWAwmHB0sHw9g2GAbzu8zzTZxvY19zRp6+WZGgKYnpzBgHmWUwV3sojKzXJ9dFm/NPJf75U+MeGAt+XrGQwdDsJZqtFb" +
            "Zn/a/GKnUdZHtY82nAgaRc2eFg4MdQDvAKi2qCLarX3WMkyWuIT5Xdk2wz0yvmE0cuRE6P+jnSAdwndLDSLpGeryPAmFuJKaCmKtAgasJNU6Dm" +
            "AYwM8BnKjIxY2AtfEQgP1O798awGZoG4P3/P7affpfPdlHRCPfJMomSdfVFxcmR3qo3wkhYdtsJps7YJQunIFuz0wcoYNDgub5aAdIzzmNxGWy" +
            "OUlXjBDsNAp/6i0wArWR70cjg+dB2tCPA3grrqSOB6ypL7X15Row0HJcecDsxNfrZCNGi2FCV6lWxb1py9Ue635Cl3eFD0g+z71MNpf0mFBO0x" +
            "weo/kby2Rzc/BfcxQ2mXdMOIJcvp7hKZ4z+N0wgPm4knqLNAudJkVHALTXDgOzyu0rhqnzY+T82ctYExZDFY06FQcWFyYl+hygOhTzaFEsqwtR" +
            "gFCrJGinzh4nGleI6E55qFXQzPoTpLnSisLNEtGGTxWWVSEbcd2Lgqes4YA3Kc8ZnuJQC5+JN+JK6o1OEY+4kjoIpzVU7vp2bd28h3D8dtjKKb" +
            "jXFv407vhqqEQwakZlwxcXJud6zYeD+noAjYJuo/z19w1hH3xEkqTZWNaIJI23VnRw3Oe5Nkoh8weC6Cs0hN6oeaHGlVRJswEz0AuhpoeIeJwF" +
            "8HJFLp6Ff2TjDTQ0Lm0Sju0vxf9uf7HTyH9Dw3gQTyQu4bDw80o/Tng6+bJmo/fGtWD0vV1cmFymfFGdaJPKib/AvhwMOEmmpRGPt+JK6lhcSQ" +
            "3DO7LxEoC3XCEbd0jHF9qPm+893OzKRA+bVSK6xGQMBgMcFosu1nLU4kqqAJfL1zM8wX6H4akKACWupE6jEdlyuiIXr7VJMoYBHAHwYjsRKWbY" +
            "Gtz6IrQ1uGv79iBufRC24uk+0YPjXXc7BJgc9cbpeREhnXsVwLyZOpls6oqYA4KiZ2bR0JCGFxcmB+javPDslVamHmqTVnKhtrgwOWHwzuriwq" +
            "RswS8gCeAovT+m698ZNMJr6yb3ptHQKsXo/ppwX8GNCBnhPUcFc2Cd3rNKJ3+4FNk0Re+ICc7GVRoTtUmeD/ELF8tkc2X9IVULHSWz3zhpCCRh" +
            "PKeEMFOpiclQk3dMkIMm71oLZ2r9fDSSrQrgVKvxE/to1l7d9eI8SwjyrdH77gnj1t2jyVkbk1PNxn6oxxa2eSYcfYEj9FHiSuo8GhVdzwPYsG" +
            "J6IbPJQTScV7/rqkZDr+AY3LqBW4PDm79+OLTduoZKOiotrfRgsbRTgrlzKpPNldrJEUAbyjSMvfRjANKZbG7CZOELizkgaMEvm9j63xPWk7qF" +
            "qrnjwrPnTd5pxd9FMWmP1r+kUd+a3BuhT5IcGSfa8ZMhma0ZJF3U+pnIZHNTaPis1Nv0/Wnan0w2V9CInUmej2ZjENY9M6F7d75dOZC85xcXJu" +
            "dg7mdiZT5qzxuncNd6O75gTdouyjdBJKImkL9yi3uSmWxuHMCYURt7inCQlmPZZDFioGcTcx0UyIT24zWDwmsdcULd2h74/Fbt4T0WL1fI6cs3" +
            "RKWlNJ3kYrrib/Pr5ZmSS+XttTwFYQBlWoSXHWxCeYEEVMknRDuBHaW/hQHkKZ9HqYVTYb6JY2FB0FgkM9lc2GyhF7QS4r3t9E3r3ylBIxSjk7" +
            "SRZmNa5+hcontrJHeNDIUBrDUhZFY2KnFDXKb3QPce7brRNshGXlcfqCrIYYrel6ZxFrVGq6RdeFKQZ80g+qmZFixvQZslRl5V6flV+t1R4d2z" +
            "mWzuSQNipIc4f2s0hldpzBNC38vt5PgwIFPau86QrCNoJOVLihqkxYXJGs1z7fp3TL57CXr+WK9rOEQtRxiMfsZwpwiGHgOf7fqL7duh/2Lx8k" +
            "RUWppbL8/M+UQ28iZawRiAtai0VFgvz0y06zCZyebGhI0qDEChU/BqK7WzsFDOCW01OtmqmWxuVXhPPpPNqU1Og+O0uFbRUGer+uiSTDanCot9" +
            "uokjYFJYc2xnE6VNQNSmjBmYC1Sj95OKW2lxb0FHShSSTc1BNs0wvUcy0FQVBOIUy2Rzc01O980287ywketP9GommytQW2IApjPZ3CkhSqQgPC" +
            "ctmLjmbCTx0kibbGBSiOjIhmxAnks0F7Xr0tTGZgR42ux5NMZrgnko3QZhVHRmpWUbfpFjJk6p+u9eMpPNxfTzo5ecRn9X2K1fPeEZgcTZc//5" +
            "1UWbJ95Z0jp4TTbmLJgg03Qd2iQdVdLcVHVq2FkAFzPZXJ4W8mab3ZTeP8LkPSuCBqNZ5FpS2NBUk2tWdQSl2enU6B6rG7liYVFvphX73YHL7F" +
            "7aWEqCbGZtDuO0bpOtNtmo6hZk1mrDh5lqnn4nWxwbOAh1LSwuTI6ZELJZHblcbhItMm8yTqYHZqPnEVEpmMw32Jwr4WbvchoBQ/OhoM8c3NOE" +
            "Ax6Vr2e4io0+6ec1AC8YLMJWkPeSdESlpbCNZEhTdH3bpIPyBEzoiIemPbjYJDtjWlwo0TrRltWFWW7mZ0CnyLpwsoy0MKfUHPhHiH0r2CEbdG" +
            "KNCO9ethHNl6a2W4VI+AotQoA1GUTsJL8Sas6gleaL5FQz29zQnpOz3IKQWIqOpPGoCbJIWti7YIH8xtqo52NX4wOb/loanu4LwsFajsDj/T7p" +
            "5wtaFM16eabuIEFdPiot5d3Y7A3IxqwNs6Or6bgXFyYLRDxGDTQ/s+QUiia5PEoWEm1ZWZjrFjf3UrNTm+53TrKkHjVZsK2exi2/m2Sj2t24aL" +
            "MKW5E/7jjcOtkcEzZlURMcLiMuTVG1ia9OQsyYa9EkZZUoqC3Ir2oSau5krpTciCDKZHPJTDY3RxrKsk6LE+4HHw6RKY5z+fpA4mof9FHWR8ys" +
            "l2eWo9LSUZtZRdPk1zG/Xp4puEA0pumkGnZQD0SFuwmKqgAmMtncPC1U2oJ4l03eYIG9ksnm4CAHiB5WI2VWBNPTlMEpdLwdZ1HdJmRXxk876E" +
            "9VmINWx1WU4zT5g7gxBmhSyHDNxjhDCAFuF+9YHCs78jYizn5DnCtn2iAaSSFUGf2a+MtIy8Epz4OJs+h9snHS5G9jDhbFCGk7LkalJSUqLcXs" +
            "kIyotJSOSktrAK7Y1Gy09Oh3K/X34sLkmO7UNRWUAndEjKpGJgI6VSecOovqT4IOwkjDDpKqXW2TFMHjAmQIeLp0u3KsB7DtdYdkI0/OqwldhM" +
            "s8fQp9kfiLy9d3FX7ew2SwGdnAenmmHpWWxprkfmhFPKYBTEelpboQI/8e7k0dHxNi491A1acIs6SFfAJSB8Z1VVhHxgV5tGtOQZcWo1v1gazK" +
            "NuddFQx4WBNJwd0h6fdEdukig/qLcBAm0KgWyAgOEdyIK6lX0MjuiV5yELWSeGy9PFONSkuSQ9JhO5mUC1D9SEZGWRbN7L+q0N+qG9kyHWyyik" +
            "AyZJ05pd5GMq2qRmYy2VzCZoRKVZejQbVZ28bqu1TciR6pe1iv44zOsVEN2Pe82swpEq39Us50uO1254qoyZsWCKSjpG49a1IRNrcqOlREh9F0" +
            "XF4GcKJHunMewLN2CszRBi51QTRV3a86RbqIiXqTU3KyQ8XXChDMKrQIx9rw3TDaxMbb8Dc4alHGTvwQ/JJ/zYXQT78IR8JilM/hgGhh3mljno" +
            "mkqeSU8Pc84aDNTfZxoEtgWB2XWQDPo3vDZK8BeKUiF5+tyEXbfSDSMRpgVXAdgORjqvVkk5P3GZM8DfA5Rbu4YCddqoIrmieSdsJIab2pCxtg" +
            "AtZzaVjeOMg3pSYQrrRHMlZ1YbsRBKs6bE1oY7hVVmsaj4QLWjC396aYhRBdmDj+XrUYcdWfhAN37L5eL5wqaVQY1knH2YpcfAbAK7SBdwtOkl" +
            "bj5XYesl6eqa2XZ0bReZ8WLXeCSv9OADjgBtnIZHPpVhsh/V0x28Ap74PouJnvwGZTEjbdhHBKVB06i4rhjlVhE1uzSjqIMKzoIjsiTbKZzrZB" +
            "kkRNl2KTGNnZ0Jd1/Qn7qI2ARV8jMYw72aJeiRuk1C0tnSjbvI0xrLUiFfSstB2n0XqvpgSvyMV6XElJQjpcLxbsCReqaaKLnPxcNbHEldSPAf" +
            "wpgD+EB5VbXSQaJypy8bybD10vz8xFpaUCpXROwF+isQJgmXKFeIFZIgk1Xc0G7eR0VF+HxMR2PyH4vWgkZl7c8Gmz1Z551YPkRiU61cZcdhYV" +
            "+xYBcI7Sd2v1UMLiYi/WDllcmJzLZHNHhcqd5yjUuETp2bXaI+JmMGHXP2JxYbJEbUoL7ylQdVhVRx4jRMjGHKjf54V6LDE0EsKtUH+qOtNQDM" +
            "BhinIySqlfE07nZZJLvVXWzFYEMZPNiTW71uj/q0L14bQu/LzgUaIttCFbwzEUIq+OCnItCVlKY5lsbk1L+67rL+xoONxgYNWKXFSBwIbKSiT0" +
            "ust2R6kiF2tt+JkUXGzPMvW120jhtYpcfJk0Hj+kzR0ByYx6AsAzFbkou002dNoOieZoweM+qQAm1sszI+vlmTmvyAZtDBFdlM0aba5lIlgi2V" +
            "hukbZcElX7dP/FTDa3ncnmtgFcpOdOe+QDsGKQPKzgYur3mi4Hyxo5vZeFfiVNNLiqoCVRNLnQ/WnRJ8dpm2lslnVtLGvyp/eVBeKccHgS1/dn" +
            "ljZH7R1X6D2KVljPgjYiJsy9fJvjJevkMK21j+bgrK7AnYxgmIT0sr1nDKn9edG8Z5BKPinMr4uC5kyyTDgqcnGObig52JBVABMVuTgKBFvTQf" +
            "08QAIsOUxopJ3WJipy8UC7ppSKXJwQNhonz9JIi0Q+K92ukTpN/XhKIB9++npoJOOHFbn4TEUuzjrx03BIPFQqmDZCJ9+SCwS5rjOVSO0mErOx" +
            "wB2gRbfapG0F8nyXLaRHP2CSHl3f1xWPbfhwkxgS6dDMa7Um645qJOfFhUmJ8ryoJvcVAIw6qdJrsNkeoOfVm6xHy041rUJ/pBZ+cWqzzZyIlV" +
            "FZATdS9cs0XkbfT20OSosLk3IHoqralW1NP7dJlmMGY6p9fw/QHFbNQtsHWjUurqQSFjQaXLfEA8SVVMRCHoWaU81Kl8pkPxpVYLWy9IdcTEZ2" +
            "nj5n/SIXsJcpNCbk13i6xaJZRcO5SwVQWy/P1AIUzy+myW47xFV8XgDDKNvpV4JO4xp5qtms7hoDEPZaJrrIl1o7/iwWQjMjTueNME/qTQrPud" +
            "G+apAIhheyEcbc1ngPgMHofhIyTOQDAL4L66GsVwGc1+qdMBgB2wTKdEqUeolIMfoXTDgYDAYjuNoNCE6eBZYMgwkHg8FgMLwgHmnciQ444JW5" +
            "gsHwA/8fRLYzlUPaOasAAAAASUVORK5CYII=";
    }
}
