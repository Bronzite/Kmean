using System;
using System.Collections.Generic;

namespace KMean
{
    static public class KMean
    {

        public static double[,] GetMeans(double[,] dNodes, int k, double convergence)
        {
            if (convergence == 0) throw new Exception("Convergence cannot be zero.");
            if (dNodes.Length == 0) return null;

            double[] dFirstNode = new double[dNodes.GetLength(1)];
            double[] dMaxValue = new double[dFirstNode.Length];
            double[] dMinValue = new double[dFirstNode.Length];

            for(int i=0;i<dMaxValue.Length;i++)
            {
                dMaxValue[i] = double.MinValue;
                dMinValue[i] = double.MaxValue;
            }
            for(int i=0;i<dNodes.GetLength(0); i++)
            {
                //Update Min/Max values for the nodes
                for (int j=0;j<dMinValue.Length;j++)
                {
                    if (dNodes[i,j] > dMaxValue[j]) dMaxValue[j] = dNodes[i,j];
                    if (dNodes[i,j] < dMinValue[j]) dMinValue[j] = dNodes[i,j];
                }
            }
            Random r = new Random();
            double[,] dMeans = new double[k,dNodes.GetLength(1)];

            //Randomly populate initial k's.
            for (int i = 0;i< k;i++)
            {
                for(int j = 0;j<dMaxValue.Length;j++)
                {
                    dMeans[i, j] = (r.NextDouble() * (dMaxValue[j] - dMinValue[j])) + dMinValue[j];
                }
            }

            double dTotalDifference = double.MaxValue;
            while (dTotalDifference > convergence * convergence)
            {
                double[,] newMeans = RecomputeMeans(dMeans, dNodes);
                double dNodeDifference = 0;
                dTotalDifference = 0;

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < dNodes.GetLength(1); j++)
                    {
                        dNodeDifference += (dMeans[i, j] - newMeans[i, j]) * (dMeans[i, j] - newMeans[i, j]);
                    }
                    dTotalDifference += Math.Sqrt(dNodeDifference);
                    

                }
                
                dMeans = newMeans;
            }
            

            return dMeans;
        }

        public static int[] AssignKValueToNode(double[,]dMeans,double[,]dNodes)
        {
            int[] iAssignedK = new int[dNodes.GetLength(0)];

            //Classify each Node with a K.
            for (int iCurrentNodeNumber = 0; iCurrentNodeNumber < dNodes.GetLength(0); iCurrentNodeNumber++)
            {
                int iBestK = int.MinValue;
                double dBestKValue = double.MaxValue;
                for (int iCurrentMean = 0; iCurrentMean < dMeans.GetLength(0); iCurrentMean++)
                {
                    double dAccumulator = 0;

                    for (int iCurrentValue = 0; iCurrentValue < dMeans.GetLength(1); iCurrentValue++)
                    {
                        dAccumulator += (dNodes[iCurrentNodeNumber, iCurrentValue] - dMeans[iCurrentMean, iCurrentValue]) * (dNodes[iCurrentNodeNumber, iCurrentValue] - dMeans[iCurrentMean, iCurrentValue]);
                    }
                    if (dAccumulator < dBestKValue) { dBestKValue = dAccumulator; iBestK = iCurrentMean; }
                }
                iAssignedK[iCurrentNodeNumber] = iBestK;
            }

            return iAssignedK;
        }

        public static double[,] ComputeMeans (double[,] dNodes, int[]iAssignedKs, int k)
        {
            double[,] retval = new double[k, dNodes.GetLength(1)];
            int[] iNodesInThisK = new int[k];

            for(int i=0;i<dNodes.GetLength(0);i++)
            {
                for(int j=0;j<dNodes.GetLength(1);j++)
                {
                    retval[iAssignedKs[i], j] += dNodes[i, j];
                    
                }
                iNodesInThisK[iAssignedKs[i]]++;
            }

            for (int i = 0; i < retval.GetLength(0); i++)
            {
                for (int j = 0; j < retval.GetLength(1); j++)
                    retval[i, j] = retval[i, j] / iNodesInThisK[i];
            }

            return retval;
        }
        

        public static double[,] RecomputeMeans (double[,] dMeans, double[,] dNodes)
        {
            int[] iAssignedK = AssignKValueToNode(dMeans, dNodes);

            double[,] newMeans = ComputeMeans(dNodes, iAssignedK, dMeans.GetLength(0));

            return newMeans;
        }

    }
}
