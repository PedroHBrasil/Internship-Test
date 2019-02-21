using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle
{
    /*
     * Contains the MF5.2 tire model coefficients of a tire.
     *  - FzO [N]: nominal normal load;
     *  - VO [m/s]: nominal speed; and
     *  - RO [m]: unloaded radius.
     */
    class TireModelMF52
    {
        // Fields
        private double ksi0 = 1, ksi1 = 1, ksi2 = 1, ksi3 = 1, ksi4 = 1, ksi5 = 1, ksi6 = 1, ksi7 = 1, ksi8 = 1,
            epsilonx = Math.Pow(10, -3), epsilony = Math.Pow(10, -1), epsilonK = Math.Pow(10, -2), epsilonV = Math.Pow(10, -3);

        // Properties ---------------------------------------------------------------------------------------------------------------------------
        // Tire Model Coefficents File
        public string FileLocation { get; set; }
        // Tire Model adimensionalization coefficients
        public double FzO { get; set; }
        public double VO { get; set; }
        public double RO { get; set; }
        // Tire Model coefficients
        public double pKy1 { get; set; }
        public double pKy2 { get; set; }
        public double pKy3 { get; set; }
        public double pKy4 { get; set; }
        public double pKy5 { get; set; }
        public double pKy6 { get; set; }
        public double pKy7 { get; set; }
        public double pVy1 { get; set; }
        public double pVy2 { get; set; }
        public double pVy3 { get; set; }
        public double pHy1 { get; set; }
        public double pHy2 { get; set; }
        public double pEy1 { get; set; }
        public double pEy2 { get; set; }
        public double pEy3 { get; set; }
        public double pEy4 { get; set; }
        public double pEy5 { get; set; }
        public double pDy1 { get; set; }
        public double pDy2 { get; set; }
        public double pDy3 { get; set; }
        public double pCy1 { get; set; }
        public double qHz1 { get; set; }
        public double qHz2 { get; set; }
        public double qHz3 { get; set; }
        public double qHz4 { get; set; }
        public double qBz1 { get; set; }
        public double qBz2 { get; set; }
        public double qBz3 { get; set; }
        public double qBz4 { get; set; }
        public double qBz5 { get; set; }
        public double qBz6 { get; set; }
        public double qBz9 { get; set; }
        public double qBz10 { get; set; }
        public double qDz1 { get; set; }
        public double qDz2 { get; set; }
        public double qDz3 { get; set; }
        public double qDz4 { get; set; }
        public double qDz6 { get; set; }
        public double qDz7 { get; set; }
        public double qDz8 { get; set; }
        public double qDz9 { get; set; }
        public double qDz10 { get; set; }
        public double qDz11 { get; set; }
        public double qEz1 { get; set; }
        public double qEz2 { get; set; }
        public double qEz3 { get; set; }
        public double qEz4 { get; set; }
        public double qEz5 { get; set; }
        public double qCz1 { get; set; }
        public double pVx1 { get; set; }
        public double pVx2 { get; set; }
        public double pHx1 { get; set; }
        public double pHx2 { get; set; }
        public double pKx1 { get; set; }
        public double pKx2 { get; set; }
        public double pKx3 { get; set; }
        public double pEx1 { get; set; }
        public double pEx2 { get; set; }
        public double pEx3 { get; set; }
        public double pEx4 { get; set; }
        public double pDx1 { get; set; }
        public double pDx2 { get; set; }
        public double pDx3 { get; set; }
        public double pCx1 { get; set; }
        public double rHx1 { get; set; }
        public double rEx1 { get; set; }
        public double rEx2 { get; set; }
        public double rCx1 { get; set; }
        public double rBx1 { get; set; }
        public double rBx2 { get; set; }
        public double rBx3 { get; set; }
        public double rVy1 { get; set; }
        public double rVy2 { get; set; }
        public double rVy3 { get; set; }
        public double rVy4 { get; set; }
        public double rVy5 { get; set; }
        public double rVy6 { get; set; }
        public double rHy1 { get; set; }
        public double rHy2 { get; set; }
        public double rEy1 { get; set; }
        public double rEy2 { get; set; }
        public double rCy1 { get; set; }
        public double rBy1 { get; set; }
        public double rBy2 { get; set; }
        public double rBy3 { get; set; }
        public double rBy4 { get; set; }
        public double ssz1 { get; set; }
        public double ssz2 { get; set; }
        public double ssz3 { get; set; }
        public double ssz4 { get; set; }
        public double qsx1 { get; set; }
        public double qsx2 { get; set; }
        public double qsx3 { get; set; }
        public double qsx4 { get; set; }
        public double qsx5 { get; set; }
        public double qsx6 { get; set; }
        public double qsx7 { get; set; }
        public double qsx8 { get; set; }
        public double qsx9 { get; set; }
        public double qsx10 { get; set; }
        public double qsx11 { get; set; }
        public double qsx12 { get; set; }
        public double qsx13 { get; set; }
        public double qsx14 { get; set; }
        public double qsy1 { get; set; }
        public double qsy2 { get; set; }
        public double qsy3 { get; set; }
        public double qsy4 { get; set; }
        public double qsy5 { get; set; }
        public double qsy6 { get; set; }
        public double qsy7 { get; set; }
        // Tire Model scaling factors
        public double lambdaFzO { get; set; }
        public double lambdaMux { get; set; }
        public double lambdaMuy { get; set; }
        public double lambdaMuV { get; set; }
        public double lambdaKxk { get; set; }
        public double lambdaKya { get; set; }
        public double lambdaCx { get; set; }
        public double lambdaCy { get; set; }
        public double lambdaEx { get; set; }
        public double lambdaEy { get; set; }
        public double lambdaHx { get; set; }
        public double lambdaHy { get; set; }
        public double lambdaVx { get; set; }
        public double lambdaVy { get; set; }
        public double lambdaKyg { get; set; }
        public double lambdaKzg { get; set; }
        public double lambdat { get; set; }
        public double lambdaMr { get; set; }
        public double lambdaxa { get; set; }
        public double lambdayk { get; set; }
        public double lambdaVyk { get; set; }
        public double lambdas { get; set; }
        public double lambdaCz { get; set; }
        public double lambdaMx { get; set; }
        public double lambdaVMx { get; set; }
        public double lambdaMy { get; set; }

        // Constructors ----------------------------------------------------------------------------------------------------------------------------
        public TireModelMF52()
        {
            // Tire model file address
            FileLocation = " ";
            // Adimensionalization coefficients
            FzO = 0; VO = 0; RO = 0;
            // Fitting coefficients
            pKy1 = 0; pKy2 = 0; pKy3 = 0; pKy4 = 0; pKy5 = 0; pKy6 = 0; pKy7 = 0; pVy1 = 0; pVy2 = 0; pVy3 = 0; pHy1 = 0; pHy2 = 0;
            pEy1 = 0; pEy2 = 0; pEy3 = 0; pEy4 = 0; pEy5 = 0; pDy1 = 0; pDy2 = 0; pDy3 = 0; pCy1 = 0; qHz1 = 0; qHz2 = 0;
            qHz3 = 0; qHz4 = 0; qBz1 = 0; qBz2 = 0; qBz3 = 0; qBz4 = 0; qBz5 = 0; qBz6 = 0; qBz9 = 0; qBz10 = 0; qDz1 = 0;
            qDz2 = 0; qDz3 = 0; qDz4 = 0; qDz6 = 0; qDz7 = 0; qDz8 = 0; qDz9 = 0; qDz10 = 0; qDz11 = 0; qEz1 = 0; qEz2 = 0;
            qEz3 = 0; qEz4 = 0; qEz5 = 0; qCz1 = 0; pVx1 = 0; pVx2 = 0; pHx1 = 0; pHx2 = 0; pKx1 = 0; pKx2 = 0; pKx3 = 0;
            pEx1 = 0; pEx2 = 0; pEx3 = 0; pEx4 = 0; pDx1 = 0; pDx2 = 0; pDx3 = 0; pCx1 = 0; rHx1 = 0; rEx1 = 0; rEx2 = 0;
            rCx1 = 0; rBx1 = 0; rBx2 = 0; rBx3 = 0; rVy1 = 0; rVy2 = 0; rVy3 = 0; rVy4 = 0; rVy5 = 0; rVy6 = 0; rHy1 = 0;
            rHy2 = 0; rEy1 = 0; rEy2 = 0; rCy1 = 0; rBy1 = 0; rBy2 = 0; rBy3 = 0; rBy4 = 0; ssz1 = 0; ssz2 = 0; ssz3 = 0;
            ssz4 = 0; qsx1 = 0; qsx2 = 0; qsx3 = 0; qsx4 = 0; qsx5 = 0; qsx6 = 0; qsx7 = 0; qsx8 = 0; qsx9 = 0; qsx10 = 0;
            qsx11 = 0; qsx12 = 0; qsx13 = 0; qsx14 = 0; qsy1 = 0; qsy2 = 0; qsy3 = 0; qsy4 = 0; qsy5 = 0; qsy6 = 0; qsy7 = 0;
            // Scaling factors
            lambdaFzO = 1; lambdaMux = 1; lambdaMuy = 1;
            lambdaMuV = 1; lambdaKxk = 1; lambdaKya = 1;
            lambdaCx = 1; lambdaCy = 1; lambdaEx = 1;
            lambdaEy = 1; lambdaHx = 1; lambdaHy = 1;
            lambdaVx = 1; lambdaVy = 1; lambdaKyg = 1;
            lambdaKzg = 1; lambdat = 1; lambdaMr = 1;
            lambdaxa = 1; lambdayk = 1; lambdaVyk = 1;
            lambdas = 1; lambdaCz = 1; lambdaMx = 1;
            lambdaVMx = 1; lambdaMy = 1;
        }

        public TireModelMF52(double FzO, double VO, double RO, double pKy1, double pKy2, double pKy3, double pKy4, double pKy5, double pKy6, double pKy7,
            double pVy1, double pVy2, double pVy3, double pHy1, double pHy2, double pEy1, double pEy2, double pEy3, double pEy4, double pEy5,
            double pDy1, double pDy2, double pDy3, double pCy1, double qHz1, double qHz2, double qHz3, double qHz4, double qBz1, double qBz2,
            double qBz3, double qBz4, double qBz5, double qBz6, double qBz9, double qBz10, double qDz1, double qDz2, double qDz3, double qDz4,
            double qDz6, double qDz7, double qDz8, double qDz9, double qDz10, double qDz11, double qEz1, double qEz2, double qEz3, double qEz4,
            double qEz5, double qCz1, double pVx1, double pVx2, double pHx1, double pHx2, double pKx1, double pKx2, double pKx3, double pEx1,
            double pEx2, double pEx3, double pEx4, double pDx1, double pDx2, double pDx3, double pCx1, double rHx1, double rEx1, double rEx2,
            double rCx1, double rBx1, double rBx2, double rBx3, double rVy1, double rVy2, double rVy3, double rVy4, double rVy5, double rVy6,
            double rHy1, double rHy2, double rEy1, double rEy2, double rCy1, double rBy1, double rBy2, double rBy3, double rBy4, double ssz1,
            double ssz2, double ssz3, double ssz4, double qsx1, double qsx2, double qsx3, double qsx4, double qsx5, double qsx6, double qsx7,
            double qsx8, double qsx9, double qsx10, double qsx11, double qsx12, double qsx13, double qsx14, double qsy1, double qsy2, double qsy3,
            double qsy4, double qsy5, double qsy6, double qsy7, double lambdaFzO, double lambdaMux, double lambdaMuy, double lambdaMuV, double lambdaKxk,
            double lambdaKya, double lambdaCx, double lambdaCy, double lambdaEx, double lambdaEy, double lambdaHx, double lambdaHy, double lambdaVx,
            double lambdaVy, double lambdaKyg, double lambdaKzg, double lambdat, double lambdaMr, double lambdaxa, double lambdayk, double lambdaVyk, double lambdas, double lambdaCz,
            double lambdaMx, double lambdaVMx, double lambdaMy)
        {
            // Tire model file address
            FileLocation = " ";
            // Adimensionalization coefficients
            this.FzO = FzO; this.VO = VO; this.RO = RO;
            // Fitting coefficients
            this.pKy1 = pKy1; this.pKy2 = pKy2; this.pKy3 = pKy3; this.pKy4 = pKy4; this.pKy5 = pKy5; this.pKy6 = pKy6; this.pKy7 = pKy7; this.pVy1 = pVy1; this.pVy2 = pVy2;
            this.pVy3 = pVy3; this.pHy1 = pHy1; this.pHy2 = pHy2; this.pEy1 = pEy1; this.pEy2 = pEy2; this.pEy3 = pEy3; this.pEy4 = pEy4; this.pEy5 = pEy5;
            this.pDy1 = pDy1; this.pDy2 = pDy2; this.pDy3 = pDy3; this.pCy1 = pCy1; this.qHz1 = qHz1; this.qHz2 = qHz2; this.qHz3 = qHz3; this.qHz4 = qHz4;
            this.qBz1 = qBz1; this.qBz2 = qBz2; this.qBz3 = qBz3; this.qBz4 = qBz4; this.qBz5 = qBz5; this.qBz6 = qBz6; this.qBz9 = qBz9; this.qBz10 = qBz10;
            this.qDz1 = qDz1; this.qDz2 = qDz2; this.qDz3 = qDz3; this.qDz4 = qDz4; this.qDz6 = qDz6; this.qDz7 = qDz7; this.qDz8 = qDz8; this.qDz9 = qDz9;
            this.qDz10 = qDz10; this.qDz11 = qDz11; this.qEz1 = qEz1; this.qEz2 = qEz2; this.qEz3 = qEz3; this.qEz4 = qEz4; this.qEz5 = qEz5; this.qCz1 = qCz1;
            this.pVx1 = pVx1; this.pVx2 = pVx2; this.pHx1 = pHx1; this.pHx2 = pHx2; this.pKx1 = pKx1; this.pKx2 = pKx2; this.pKx3 = pKx3; this.pEx1 = pEx1;
            this.pEx2 = pEx2; this.pEx3 = pEx3; this.pEx4 = pEx4; this.pDx1 = pDx1; this.pDx2 = pDx2; this.pDx3 = pDx3; this.pCx1 = pCx1; this.rHx1 = rHx1;
            this.rEx1 = rEx1; this.rEx2 = rEx2; this.rCx1 = rCx1; this.rBx1 = rBx1; this.rBx2 = rBx2; this.rBx3 = rBx3; this.rVy1 = rVy1; this.rVy2 = rVy2;
            this.rVy3 = rVy3; this.rVy4 = rVy4; this.rVy5 = rVy5; this.rVy6 = rVy6; this.rHy1 = rHy1; this.rHy2 = rHy2; this.rEy1 = rEy1; this.rEy2 = rEy2;
            this.rCy1 = rCy1; this.rBy1 = rBy1; this.rBy2 = rBy2; this.rBy3 = rBy3; this.rBy4 = rBy4; this.ssz1 = ssz1; this.ssz2 = ssz2; this.ssz3 = ssz3;
            this.ssz4 = ssz4; this.qsx1 = qsx1; this.qsx2 = qsx2; this.qsx3 = qsx3; this.qsx4 = qsx4; this.qsx5 = qsx5; this.qsx6 = qsx6; this.qsx7 = qsx7;
            this.qsx8 = qsx8; this.qsx9 = qsx9; this.qsx10 = qsx10; this.qsx11 = qsx11; this.qsx12 = qsx12; this.qsx13 = qsx13; this.qsx14 = qsx14;
            this.qsy1 = qsy1; this.qsy2 = qsy2; this.qsy3 = qsy3; this.qsy4 = qsy4; this.qsy5 = qsy5; this.qsy6 = qsy6; this.qsy7 = qsy7;
            // Scaling factors
            this.lambdaFzO = lambdaFzO; this.lambdaMux = lambdaMux; this.lambdaMuy = lambdaMuy;
            this.lambdaMuV = lambdaMuV; this.lambdaKxk = lambdaKxk; this.lambdaKya = lambdaKya;
            this.lambdaCx = lambdaCx; this.lambdaCy = lambdaCy; this.lambdaEx = lambdaEx;
            this.lambdaEy = lambdaEy; this.lambdaHx = lambdaHx; this.lambdaHy = lambdaHy;
            this.lambdaVx = lambdaVx; this.lambdaVy = lambdaVy; this.lambdaKyg = lambdaKyg;
            this.lambdaKzg = lambdaKzg; this.lambdat = lambdat; this.lambdaMr = lambdaMr;
            this.lambdaxa = lambdaxa; this.lambdayk = lambdayk; this.lambdaVyk = lambdaVyk;
            this.lambdas = lambdas; this.lambdaCz = lambdaCz; this.lambdaMx = lambdaMx;
            this.lambdaVMx = lambdaVMx; this.lambdaMy = lambdaMy;
        }

        public TireModelMF52(string fileLocation, List<double> lambdaList)
        {
            // Tire Model file address
            FileLocation = fileLocation;

            // Reads the coefficients in the .txt file
            var fileArray = File.ReadAllLines(fileLocation);
            // Loop for writing of the coefficients
            foreach (var line in File.ReadLines(fileLocation))
            {
                // Splits the line betweencoefficient name [0] and coefficient value [1]
                var tempLine = line.Split('\t');
                // Verifies the coefficients names and assign the values accordingly 
                switch (tempLine[0])
                {
                    // Adimensionallization coefficients
                    case "FzO": { FzO = double.Parse(tempLine[1]); continue; }
                    case "VO": { VO = double.Parse(tempLine[1]); continue; }
                    case "RO": { RO = double.Parse(tempLine[1]); continue; }
                    // Fitting coefficients
                    case "pKy1": { pKy1 = double.Parse(tempLine[1]); continue; }
                    case "pKy2": { pKy2 = double.Parse(tempLine[1]); continue; }
                    case "pKy3": { pKy3 = double.Parse(tempLine[1]); continue; }
                    case "pKy4": { pKy4 = double.Parse(tempLine[1]); continue; }
                    case "pKy5": { pKy5 = double.Parse(tempLine[1]); continue; }
                    case "pKy6": { pKy6 = double.Parse(tempLine[1]); continue; }
                    case "pKy7": { pKy7 = double.Parse(tempLine[1]); continue; }
                    case "pVy1": { pVy1 = double.Parse(tempLine[1]); continue; }
                    case "pVy2": { pVy2 = double.Parse(tempLine[1]); continue; }
                    case "pVy3": { pVy3 = double.Parse(tempLine[1]); continue; }
                    case "pHy1": { pHy1 = double.Parse(tempLine[1]); continue; }
                    case "pHy2": { pHy2 = double.Parse(tempLine[1]); continue; }
                    case "pEy1": { pEy1 = double.Parse(tempLine[1]); continue; }
                    case "pEy2": { pEy2 = double.Parse(tempLine[1]); continue; }
                    case "pEy3": { pEy3 = double.Parse(tempLine[1]); continue; }
                    case "pEy4": { pEy4 = double.Parse(tempLine[1]); continue; }
                    case "pEy5": { pEy5 = double.Parse(tempLine[1]); continue; }
                    case "pDy1": { pDy1 = double.Parse(tempLine[1]); continue; }
                    case "pDy2": { pDy2 = double.Parse(tempLine[1]); continue; }
                    case "pDy3": { pDy3 = double.Parse(tempLine[1]); continue; }
                    case "pCy1": { pCy1 = double.Parse(tempLine[1]); continue; }
                    case "qHz1": { qHz1 = double.Parse(tempLine[1]); continue; }
                    case "qHz2": { qHz2 = double.Parse(tempLine[1]); continue; }
                    case "qHz3": { qHz3 = double.Parse(tempLine[1]); continue; }
                    case "qHz4": { qHz4 = double.Parse(tempLine[1]); continue; }
                    case "qBz1": { qBz1 = double.Parse(tempLine[1]); continue; }
                    case "qBz2": { qBz2 = double.Parse(tempLine[1]); continue; }
                    case "qBz3": { qBz3 = double.Parse(tempLine[1]); continue; }
                    case "qBz4": { qBz4 = double.Parse(tempLine[1]); continue; }
                    case "qBz5": { qBz5 = double.Parse(tempLine[1]); continue; }
                    case "qBz6": { qBz6 = double.Parse(tempLine[1]); continue; }
                    case "qBz9": { qBz9 = double.Parse(tempLine[1]); continue; }
                    case "qBz10": { qBz10 = double.Parse(tempLine[1]); continue; }
                    case "qDz1": { qDz1 = double.Parse(tempLine[1]); continue; }
                    case "qDz2": { qDz2 = double.Parse(tempLine[1]); continue; }
                    case "qDz3": { qDz3 = double.Parse(tempLine[1]); continue; }
                    case "qDz4": { qDz4 = double.Parse(tempLine[1]); continue; }
                    case "qDz6": { qDz6 = double.Parse(tempLine[1]); continue; }
                    case "qDz7": { qDz7 = double.Parse(tempLine[1]); continue; }
                    case "qDz8": { qDz8 = double.Parse(tempLine[1]); continue; }
                    case "qDz9": { qDz9 = double.Parse(tempLine[1]); continue; }
                    case "qDz10": { qDz10 = double.Parse(tempLine[1]); continue; }
                    case "qDz11": { qDz11 = double.Parse(tempLine[1]); continue; }
                    case "qEz1": { qEz1 = double.Parse(tempLine[1]); continue; }
                    case "qEz2": { qEz2 = double.Parse(tempLine[1]); continue; }
                    case "qEz3": { qEz3 = double.Parse(tempLine[1]); continue; }
                    case "qEz4": { qEz4 = double.Parse(tempLine[1]); continue; }
                    case "qEz5": { qEz5 = double.Parse(tempLine[1]); continue; }
                    case "qCz1": { qCz1 = double.Parse(tempLine[1]); continue; }
                    case "pVx1": { pVx1 = double.Parse(tempLine[1]); continue; }
                    case "pVx2": { pVx2 = double.Parse(tempLine[1]); continue; }
                    case "pHx1": { pHx1 = double.Parse(tempLine[1]); continue; }
                    case "pHx2": { pHx2 = double.Parse(tempLine[1]); continue; }
                    case "pKx1": { pKx1 = double.Parse(tempLine[1]); continue; }
                    case "pKx2": { pKx2 = double.Parse(tempLine[1]); continue; }
                    case "pKx3": { pKx3 = double.Parse(tempLine[1]); continue; }
                    case "pEx1": { pEx1 = double.Parse(tempLine[1]); continue; }
                    case "pEx2": { pEx2 = double.Parse(tempLine[1]); continue; }
                    case "pEx3": { pEx3 = double.Parse(tempLine[1]); continue; }
                    case "pEx4": { pEx4 = double.Parse(tempLine[1]); continue; }
                    case "pDx1": { pDx1 = double.Parse(tempLine[1]); continue; }
                    case "pDx2": { pDx2 = double.Parse(tempLine[1]); continue; }
                    case "pDx3": { pDx3 = double.Parse(tempLine[1]); continue; }
                    case "pCx1": { pCx1 = double.Parse(tempLine[1]); continue; }
                    case "rHx1": { rHx1 = double.Parse(tempLine[1]); continue; }
                    case "rEx1": { rEx1 = double.Parse(tempLine[1]); continue; }
                    case "rEx2": { rEx2 = double.Parse(tempLine[1]); continue; }
                    case "rCx1": { rCx1 = double.Parse(tempLine[1]); continue; }
                    case "rBx1": { rBx1 = double.Parse(tempLine[1]); continue; }
                    case "rBx2": { rBx2 = double.Parse(tempLine[1]); continue; }
                    case "rBx3": { rBx3 = double.Parse(tempLine[1]); continue; }
                    case "rVy1": { rVy1 = double.Parse(tempLine[1]); continue; }
                    case "rVy2": { rVy2 = double.Parse(tempLine[1]); continue; }
                    case "rVy3": { rVy3 = double.Parse(tempLine[1]); continue; }
                    case "rVy4": { rVy4 = double.Parse(tempLine[1]); continue; }
                    case "rVy5": { rVy5 = double.Parse(tempLine[1]); continue; }
                    case "rVy6": { rVy6 = double.Parse(tempLine[1]); continue; }
                    case "rHy1": { rHy1 = double.Parse(tempLine[1]); continue; }
                    case "rHy2": { rHy2 = double.Parse(tempLine[1]); continue; }
                    case "rEy1": { rEy1 = double.Parse(tempLine[1]); continue; }
                    case "rEy2": { rEy2 = double.Parse(tempLine[1]); continue; }
                    case "rCy1": { rCy1 = double.Parse(tempLine[1]); continue; }
                    case "rBy1": { rBy1 = double.Parse(tempLine[1]); continue; }
                    case "rBy2": { rBy2 = double.Parse(tempLine[1]); continue; }
                    case "rBy3": { rBy3 = double.Parse(tempLine[1]); continue; }
                    case "rBy4": { rBy4 = double.Parse(tempLine[1]); continue; }
                    case "ssz1": { ssz1 = double.Parse(tempLine[1]); continue; }
                    case "ssz2": { ssz2 = double.Parse(tempLine[1]); continue; }
                    case "ssz3": { ssz3 = double.Parse(tempLine[1]); continue; }
                    case "ssz4": { ssz4 = double.Parse(tempLine[1]); continue; }
                    case "qsx1": { qsx1 = double.Parse(tempLine[1]); continue; }
                    case "qsx2": { qsx2 = double.Parse(tempLine[1]); continue; }
                    case "qsx3": { qsx3 = double.Parse(tempLine[1]); continue; }
                    case "qsx4": { qsx4 = double.Parse(tempLine[1]); continue; }
                    case "qsx5": { qsx5 = double.Parse(tempLine[1]); continue; }
                    case "qsx6": { qsx6 = double.Parse(tempLine[1]); continue; }
                    case "qsx7": { qsx7 = double.Parse(tempLine[1]); continue; }
                    case "qsx8": { qsx8 = double.Parse(tempLine[1]); continue; }
                    case "qsx9": { qsx9 = double.Parse(tempLine[1]); continue; }
                    case "qsx10": { qsx10 = double.Parse(tempLine[1]); continue; }
                    case "qsx11": { qsx11 = double.Parse(tempLine[1]); continue; }
                    case "qsx12": { qsx12 = double.Parse(tempLine[1]); continue; }
                    case "qsx13": { qsx13 = double.Parse(tempLine[1]); continue; }
                    case "qsx14": { qsx14 = double.Parse(tempLine[1]); continue; }
                    case "qsy1": { qsy1 = double.Parse(tempLine[1]); continue; }
                    case "qsy2": { qsy2 = double.Parse(tempLine[1]); continue; }
                    case "qsy3": { qsy3 = double.Parse(tempLine[1]); continue; }
                    case "qsy4": { qsy4 = double.Parse(tempLine[1]); continue; }
                    case "qsy5": { qsy5 = double.Parse(tempLine[1]); continue; }
                    case "qsy6": { qsy6 = double.Parse(tempLine[1]); continue; }
                    case "qsy7": { qsy7 = double.Parse(tempLine[1]); continue; }
                    default: continue;
                }
            }
            // Scaling factors
            lambdaFzO = lambdaList[0]; lambdaMux = lambdaList[1]; lambdaMuy = lambdaList[2];
            lambdaMuV = lambdaList[3]; lambdaKxk = lambdaList[4]; lambdaKya = lambdaList[5];
            lambdaCx = lambdaList[6]; lambdaCy = lambdaList[7]; lambdaEx = lambdaList[8];
            lambdaEy = lambdaList[9]; lambdaHx = lambdaList[10]; lambdaHy = lambdaList[11];
            lambdaVx = lambdaList[12]; lambdaVy = lambdaList[13]; lambdaKyg = lambdaList[14];
            lambdaKzg = lambdaList[15]; lambdat = lambdaList[16]; lambdaMr = lambdaList[17];
            lambdaxa = lambdaList[18]; lambdayk = lambdaList[19]; lambdaVyk = lambdaList[20];
            lambdas = lambdaList[21]; lambdaCz = lambdaList[22]; lambdaMx = lambdaList[23];
            lambdaVMx = lambdaList[24]; lambdaMy = lambdaList[25];
        }

        // Methods
        public double GetTireFx(double kappa, double alpha, double Fz, double gamma, double Vc)
        {
            // Speed components
            double Vcx = Vc * Math.Cos(alpha);
            double Vsx = -kappa * Math.Abs(Vcx);
            // Adimensionalization
            double FzOLine = lambdaFzO * FzO;
            double dfz = (Fz - FzOLine) / FzOLine;
            double alphaStar = Math.Tan(alpha) * Math.Sign(Vcx);
            double gammaStar = Math.Sin(gamma);
            // Calculated Scalling factors
            double Amu = 10;
            double lambdaMuxStar = lambdaMux / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuyStar = lambdaMuy / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuxLine = Amu * lambdaMuxStar / (1 + (Amu - 1) * lambdaMuxStar);
            double lambdaMuyLine = Amu * lambdaMuyStar / (1 + (Amu - 1) * lambdaMuyStar);
            // Pure Slip Magic Formula
            double SVx = Fz * (pVx1 + pVx2 * dfz) * lambdaVx * lambdaMuxLine * ksi1;
            double SHx = (pHx1 + pHx2 * dfz) * lambdaHx;
            double kappax = kappa + SHx;
            double mux = (pDx1 + pDx2 * dfz) * (1 - pDx3 * Math.Pow(gamma, 2)) * lambdaMuxStar;
            double Ex = (pEx1 + pEx2 * dfz + pEx3 * Math.Pow(dfz, 2)) * (1 - pEx4 * Math.Sign(kappax)) * lambdaMuxStar;
            double Dx = mux * Fz * ksi1;
            double Cx = pCx1 * lambdaCx;
            double KxKappa = Fz * (pKx1 + pKx2 * dfz) * Math.Exp(pKx3 * dfz) * lambdaKxk;
            double Bx = KxKappa / (Cx * Dx + epsilonx);
            double FxO = Dx * Math.Sin(Cx * Math.Atan(Bx * kappax - Ex * (Bx * kappax - Math.Atan(Bx * kappax)))) + SVx;
            // Combined Magic Formula
            double SHxAlpha = rHx1;
            double ExAlpha = rEx1 + rEx2 * dfz;
            double CxAlpha = rCx1;
            double BxAlpha = (rBx1 + rBx3 * Math.Pow(gammaStar, 2)) * Math.Cos(Math.Atan(rBx2 * kappa)) * lambdaxa;
            double alphaS = alphaStar + SHxAlpha;
            double GxAlphaO = Math.Cos(CxAlpha * Math.Atan(BxAlpha * SHxAlpha - ExAlpha * (BxAlpha * SHxAlpha - Math.Atan(BxAlpha * SHxAlpha))));
            double GxAlpha = Math.Cos(CxAlpha * Math.Atan(BxAlpha * alphaS - ExAlpha * (BxAlpha * alphaS - Math.Atan(BxAlpha * alphaS)))) / GxAlphaO;
            double Fx = GxAlphaO * FxO;

            return Fx;
        }

        public double GetTireFy(double kappa, double alpha, double Fz, double gamma, double Vc)
        {
            // Speed components
            double Vcx = Vc * Math.Cos(alpha);
            double Vsx = -kappa * Math.Abs(Vcx);
            // Adimensionalization
            double FzOLine = lambdaFzO * FzO;
            double dfz = (Fz - FzOLine) / FzOLine;
            double alphaStar = Math.Tan(alpha) * Math.Sign(Vcx);
            double gammaStar = Math.Sin(gamma);
            // Calculated Scalling factors
            double Amu = 10;
            double lambdaMuxStar = lambdaMux / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuyStar = lambdaMuy / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuxLine = Amu * lambdaMuxStar / (1 + (Amu - 1) * lambdaMuxStar);
            double lambdaMuyLine = Amu * lambdaMuyStar / (1 + (Amu - 1) * lambdaMuyStar);
            // Pure Slip Magic Formula
            double KyGammaO = Fz * (pKy6 + pKy7 * dfz) * lambdaKyg;
            double SVyGamma = Fz * (pVy3 + pVy2 * Fz) * gammaStar * lambdaKyg * lambdaMuyLine * ksi2;
            double SVy = Fz * (pVy1 + pVy2 * dfz) * lambdaVy * lambdaMuyLine * ksi2 + SVyGamma;
            double KyAlpha = pKy1 * FzOLine * (1 - pKy3 * Math.Abs(gammaStar) * Math.Sin(pKy4 * Math.Atan(Fz / (FzOLine * pKy2 + pKy5 * Math.Pow(gammaStar, 2))))) * ksi3 * lambdaKya;
            double SHy = (pHy1 + pHy2 * dfz) * lambdaHy + (KyGammaO * gammaStar - SVyGamma) / (KyAlpha + epsilonK) * ksi0 + ksi4 - 1;
            double muy = (pDy1 + pDy2 * dfz) * (1 - pDy3 * Math.Pow(gammaStar, 2)) * lambdaMuyStar;
            double alphay = alphaStar + SHy;
            double Ey = (pEy1 + pEy2 * dfz) * (1 + pEy5 * Math.Pow(gammaStar, 2) - (pEy3 + pEy4 * gammaStar) * Math.Sign(alphay)) * lambdaEy;
            double Dy = muy * Fz * ksi2;
            double Cy = pCy1 * lambdaCy;
            double By = KyAlpha / (Cy * Dy + epsilony);
            double FyO = Dy * Math.Sin(Cy * Math.Atan(By * alphay - Ey * (By * alphay - Math.Atan(By * alphay)))) + SVy;
            // Combined Magic Formula
            double DVyKappa = muy * Fz * (rVy1 + rVy2 * dfz + rVy3 * gammaStar) * Math.Cos(Math.Atan(rVy4 * alphaStar)) * ksi2;
            double SVyKappa = DVyKappa * Math.Sin(rVy5 * Math.Atan(rVy6 * kappa)) * lambdaVyk;
            double SHyKappa = rHy1 + rHy2 * dfz;
            double EyKappa = rEy1 + rEy2 * dfz;
            double CyKappa = rCy1;
            double ByKappa = (rBy1 + rBy2 * Math.Pow(gammaStar, 2)) * Math.Cos(Math.Atan(rBy2 * (alphaStar - rBy3))) * lambdayk;
            double kappaS = kappa + SHyKappa;
            double GyKappaO = Math.Cos(CyKappa * Math.Atan(ByKappa * SHyKappa - EyKappa * (ByKappa * SHyKappa - Math.Atan(ByKappa * SHyKappa))));
            double GyKappa = Math.Cos(CyKappa * Math.Atan(ByKappa * kappaS - EyKappa * (ByKappa * kappaS - Math.Atan(ByKappa * kappaS)))) / GyKappaO;
            double Fy = GyKappa * FyO + SVyKappa;

            return Fy;
        }

        public double GetTireMx(double kappa, double alpha, double Fz, double gamma, double Vc)
        {
            // Speed components
            double Vcx = Vc * Math.Cos(alpha);
            double Vsx = -kappa * Math.Abs(Vcx);
            // Adimensionalization
            double FzOLine = lambdaFzO * FzO;
            double dfz = (Fz - FzOLine) / FzOLine;
            double alphaStar = Math.Tan(alpha) * Math.Sign(Vcx);
            double gammaStar = Math.Sin(gamma);
            // Calculated Scalling factors
            double Amu = 10;
            double lambdaMuxStar = lambdaMux / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuyStar = lambdaMuy / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuxLine = Amu * lambdaMuxStar / (1 + (Amu - 1) * lambdaMuxStar);
            double lambdaMuyLine = Amu * lambdaMuyStar / (1 + (Amu - 1) * lambdaMuyStar);
            // Lateral Force
            double Fy = GetTireFy(kappa, alpha, Fz, gamma, Vc);
            // Combined Magic Formula
            double Mx = RO * Fz * (qsx1 * lambdaVMx - qsx2 * gamma + qsx3 * Fy / FzO + qsx4 *
                Math.Cos(qsx5 * Math.Pow(Math.Atan(qsx6 * Fz / FzO), 2))) *
                Math.Sin(qsx7 * gamma + qsx8 * Math.Atan(qsx9 * Fy / FzO) + qsx10 *
                Math.Atan(qsx11 * Fz / FzO) * gamma) * lambdaMx;

            return Mx;
        }

        public double GetTireMy(double kappa, double alpha, double Fz, double gamma, double Vc)
        {
            // Speed components
            double Vcx = Vc * Math.Cos(alpha);
            double Vsx = -kappa * Math.Abs(Vcx);
            // Adimensionalization
            double FzOLine = lambdaFzO * FzO;
            double dfz = (Fz - FzOLine) / FzOLine;
            double alphaStar = Math.Tan(alpha) * Math.Sign(Vcx);
            double gammaStar = Math.Sin(gamma);
            // Calculated Scalling factors
            double Amu = 10;
            double lambdaMuxStar = lambdaMux / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuyStar = lambdaMuy / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuxLine = Amu * lambdaMuxStar / (1 + (Amu - 1) * lambdaMuxStar);
            double lambdaMuyLine = Amu * lambdaMuyStar / (1 + (Amu - 1) * lambdaMuyStar);
            // Longitudinal Force
            double Fx = GetTireFx(kappa, alpha, Fz, gamma, Vc);
            // Combined Magic Formula
            double My = Fz * RO * (qsy1 + qsy2 * Fx / (FzO + qsy3 * Math.Abs(Vcx / VO) + qsy4 *
                Math.Pow(Vcx / VO, 4) + qsy5 + qsy6 * Fz / FzO * Math.Pow(gamma, 2)) *
                Math.Pow(Fz / FzO, qsy7 * lambdaMy));

            return My;
        }
        public double GetTireMz(double kappa, double alpha, double Fz, double gamma, double Vc)
        {
            // Speed components
            double Vcx = Vc * Math.Cos(alpha);
            double Vsx = -kappa * Math.Abs(Vcx);
            // Adimensionalization
            double FzOLine = lambdaFzO * FzO;
            double dfz = (Fz - FzOLine) / FzOLine;
            double alphaStar = Math.Tan(alpha) * Math.Sign(Vcx);
            double gammaStar = Math.Sin(gamma);
            double VcLine = Vc + epsilonV;
            double cosAlphaLine = Vcx / VcLine;
            // Calculated Scalling factors
            double Amu = 10;
            double lambdaMuxStar = lambdaMux / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuyStar = lambdaMuy / (1 + lambdaMuV * (Vsx / (Math.Cos(alpha))) / VO);
            double lambdaMuxLine = Amu * lambdaMuxStar / (1 + (Amu - 1) * lambdaMuxStar);
            double lambdaMuyLine = Amu * lambdaMuyStar / (1 + (Amu - 1) * lambdaMuyStar);

            // Pure Slip Magic Formula (Fx)
            double SVx = Fz * (pVx1 + pVx2 * dfz) * lambdaVx * lambdaMuxLine * ksi1;
            double SHx = (pHx1 + pHx2 * dfz) * lambdaHx;
            double kappax = kappa + SHx;
            double mux = (pDx1 + pDx2 * dfz) * (1 - pDx3 * Math.Pow(gamma, 2)) * lambdaMuxStar;
            double Ex = (pEx1 + pEx2 * dfz + pEx3 * Math.Pow(dfz, 2)) * (1 - pEx4 * Math.Sign(kappax)) * lambdaMuxStar;
            double Dx = mux * Fz * ksi1;
            double Cx = pCx1 * lambdaCx;
            double KxKappa = Fz * (pKx1 + pKx2 * dfz) * Math.Exp(pKx3 * dfz) * lambdaKxk;
            double Bx = KxKappa / (Cx * Dx + epsilonx);
            double FxO = Dx * Math.Sin(Cx * Math.Atan(Bx * kappax - Ex * (Bx * kappax - Math.Atan(Bx - kappax)))) + SVx;
            // Combined Magic Formula (Fx)
            double SHxAlpha = rHx1;
            double ExAlpha = rEx1 + rEx2 * dfz;
            double CxAlpha = rCx1;
            double BxAlpha = (rBx1 + rBx3 * Math.Pow(gammaStar, 2)) * Math.Cos(Math.Atan(rBx2 * kappa)) * lambdaxa;
            double alphaS = alphaStar + SHxAlpha;
            double GxAlphaO = Math.Cos(CxAlpha * Math.Atan(BxAlpha * SHxAlpha - ExAlpha * (BxAlpha * SHxAlpha - Math.Atan(BxAlpha * SHxAlpha))));
            double GxAlpha = Math.Cos(CxAlpha * Math.Atan(BxAlpha * alphaS - ExAlpha * (BxAlpha * alphaS - Math.Atan(BxAlpha * alphaS)))) / GxAlphaO;
            double Fx = GxAlphaO * FxO;

            // Pure Slip Magic Formula (Fy)
            double KyGammaO = Fz * (pKy6 + pKy7 * dfz) * lambdaKyg;
            double SVyGamma = Fz * (pVy3 + pVy2 * Fz) * gammaStar * lambdaKyg * lambdaMuyLine * ksi2;
            double SVy = Fz * (pVy1 + pVy2 * dfz) * lambdaVy * lambdaMuyLine * ksi2 + SVyGamma;
            double KyAlpha = pKy1 * FzOLine * (1 - pKy3 * Math.Abs(gammaStar) * Math.Sin(pKy4 * Math.Atan(Fz / (FzOLine * pKy2 + pKy5 * Math.Pow(gammaStar, 2))))) * ksi3 * lambdaKya;
            double SHy = (pHy1 + pHy2 * dfz) * lambdaHy + (KyGammaO * gammaStar - SVyGamma) / (KyAlpha + epsilonK) * ksi0 + ksi4 - 1;
            double muy = (pDy1 + pDy2 * dfz) * (1 - pDy3 * Math.Pow(gammaStar, 2)) * lambdaMuyStar;
            double alphay = alphaStar + SHy;
            double Ey = (pEy1 + pEy2 * dfz) * (1 + pEy5 * Math.Pow(gammaStar, 2) - (pEy3 + pEy4 * gammaStar) * Math.Sign(alphay)) * lambdaEy;
            double Dy = muy * Fz * ksi2;
            double Cy = pCy1 * lambdaCy;
            double By = KyAlpha / (Cy * Dy + epsilony);
            double FyO = Dy * Math.Sin(Cy * Math.Atan(By * alphay - Ey * (By * alphay - Math.Atan(By * alphay)))) + SVy;
            // Combined Magic Formula (Fy)
            double DVyKappa = muy * Fz * (rVy1 + rVy2 * dfz + rVy3 * gammaStar) * Math.Cos(Math.Atan(rVy4 * alphaStar)) * ksi2;
            double SVyKappa = DVyKappa * Math.Sin(rVy5 * Math.Atan(rVy6 * kappa)) * lambdaVyk;
            double SHyKappa = rHy1 + rHy2 * dfz;
            double EyKappa = rEy1 + rEy2 * dfz;
            double CyKappa = rCy1;
            double ByKappa = (rBy1 + rBy2 * Math.Pow(gammaStar, 2)) * Math.Cos(Math.Atan(rBy2 * (alphaStar - rBy3))) * lambdayk;
            double kappaS = kappa + SHyKappa;
            double GyKappaO = Math.Cos(CyKappa * Math.Atan(ByKappa * SHyKappa - EyKappa * (ByKappa * SHyKappa - Math.Atan(ByKappa * SHyKappa))));
            double GyKappa = Math.Cos(CyKappa * Math.Atan(ByKappa * kappaS - EyKappa * (ByKappa * kappaS - Math.Atan(ByKappa * kappaS)))) / GyKappaO;
            double Fy = GyKappa * FyO + SVyKappa;

            // Pure Slip Magic Formula (Fy, gamma=0)
            double KyAlpha_gamma0 = pKy1 * FzOLine * ksi3 * lambdaKya;
            double SHy_gamma0 = (pHy1 + pHy2 * dfz) * lambdaHy * ksi0 + ksi4 - 1;
            double muy_gamma0 = (pDy1 + pDy2 * dfz) * lambdaMuyStar;
            double alphay_gamma0 = alphaStar + SHy_gamma0;
            double Ey_gamma0 = (pEy1 + pEy2 * dfz) * lambdaEy;
            double Dy_gamma0 = muy_gamma0 * Fz * ksi2;
            double By_gamma0 = KyAlpha_gamma0 / (Cy * Dy_gamma0 + epsilony);
            double FyO_gamma0 = Dy_gamma0 * Math.Sin(Cy * Math.Atan(By_gamma0 * alphay - Ey_gamma0 * (By_gamma0 * alphay_gamma0 - Math.Atan(By_gamma0 * alphay_gamma0)))) + SVy;
            // Combined Magic Formula (Fy, gamma=0)
            double DVyKappa_gamma0 = muy_gamma0 * Fz * (rVy1 + rVy2 * dfz) * Math.Cos(Math.Atan(rVy4 * alphaStar)) * ksi2;
            double SVyKappa_gamma0 = DVyKappa_gamma0 * Math.Sin(rVy5 * Math.Atan(rVy6 * kappa)) * lambdaVyk;
            double ByKappa_gamma0 = rBy1 * Math.Cos(Math.Atan(rBy2 * (alphaStar - rBy3))) * lambdayk;
            double GyKappaO_gamma0 = Math.Cos(CyKappa * Math.Atan(ByKappa_gamma0 * SHyKappa - EyKappa * (ByKappa_gamma0 * SHyKappa - Math.Atan(ByKappa_gamma0 * SHyKappa))));
            double GyKappa_gamma0 = Math.Cos(CyKappa * Math.Atan(ByKappa_gamma0 * kappaS - EyKappa * (ByKappa_gamma0 * kappaS - Math.Atan(ByKappa_gamma0 * kappaS)))) / GyKappaO_gamma0;
            double Fy_gamma0 = GyKappa_gamma0 * FyO_gamma0 + SVyKappa_gamma0;

            // Pure Slip Magic Formula (Mz)
            double Dr = Fz * RO * ((qDz6 + qDz7 * dfz) * lambdaMr * ksi2 + ((qDz8 + qDz9 * dfz) + (qDz10 + qDz11 * dfz) * Math.Abs(gammaStar)) * gammaStar * lambdaKzg * ksi0) * lambdaMuyStar * Math.Sign(Vcx) * cosAlphaLine + ksi8 - 1;
            double Cr = ksi7;
            double Br = (qBz9 * KyAlpha / lambdaMuyStar + qBz10 * By * Cy) * ksi6;
            double SHt = qHz1 + qHz2 * dfz + (qHz3 + qHz4 * dfz) * gammaStar;
            double alphat = alphaStar + SHt;
            double KyAlphaLine = KyAlpha + epsilonK;
            double SHf = SHy + SVy / KyAlphaLine;
            double alphar = alphaStar + SHf;
            double Bt = (qBz1 + qBz2 * dfz + qBz3 * Math.Pow(dfz, 2)) * (1 + qBz4 + qBz5) * Math.Abs(gammaStar) + qBz6 * Math.Pow(gammaStar, 2) * lambdaKya * lambdaMuyStar;
            double Ct = qCz1;
            double DtO = Fz * (RO / FzOLine) * (qDz1 + qDz2 * dfz) * lambdat * Math.Sign(Vcx);
            double Dt = DtO * (1 + qDz3 * Math.Abs(gammaStar) + qDz4 * Math.Pow(gammaStar, 2)) * ksi5;
            double Et = qEz1 + qEz2 * dfz + qEz3 * Math.Pow(dfz, 2) * (1 + (qEz4 + qEz5 * gammaStar) * 2 / Math.PI * Math.Atan(Bt * Ct * alphat));
            // Combined Magic Formula (Mz)
            double alphareq = Math.Sqrt(Math.Pow(alphar, 2) + Math.Pow(KxKappa / KyAlphaLine, 2) * Math.Pow(kappa, 2)) * Math.Sign(alphar);
            double alphateq = Math.Sqrt(Math.Pow(alphat, 2) + Math.Pow(KxKappa / KyAlphaLine, 2) * Math.Pow(kappa, 2)) * Math.Sign(alphat);
            double s = RO * (ssz1 + ssz2 * (Fy / FzOLine) + (ssz3 + ssz4 * dfz) * gammaStar) * lambdas;
            double Mzr = Dr * Math.Cos(Cr * Math.Atan(Br * alphareq));
            double FyLine = GyKappaO_gamma0 * FyO_gamma0;
            double t = Dt * Math.Cos(Ct * Math.Atan(Bt * alphateq - Et * (Bt * alphateq - Math.Atan(Bt * alphateq)))) * cosAlphaLine;
            double MzLine = -t * FyLine;
            double Mz = MzLine + Mzr + s * Fx;

            return Mz;
        }
    }
}
