using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();

        }

        ///////////// INPUTS ///////////// 

        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }

        //==================Variables and Diff=====================================
        public static Random r = new Random();
        public decimal ComProb = 0;
        public decimal IntComProb = 0;
        int min = 0;
        int Imin = 0;
        public int countofcustomer = 0;
        public int randomTime = r.Next(1, 10);
        public int counterToAddInQueue = 0;
        public Queue<SimulationCase> QSC = new Queue<SimulationCase>();
        public SimulationSystem ssss;
        public int shrouk = 0;
        public int T = 0;
        public SimulationSystem sepcserver;
        //==========================================================================

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        //=========================sERVER CREATION===============================================
        public void server_creation(int x)
        {
            int j = 0;
            for (int i = 1; i <= x; i++)
            {
                Servers.Add(new Server());
                Servers[j].ID = i;
                j++;
            }
        }
        //=================================================================================================
        //===================TIME,PROBABILITY AND COMMULATIVE PROB=========================================
        public void time_and_prob(int t, decimal prob, int ser, int rows, int listOfTime)
        {

            Servers[ser].TimeDistribution.Add(new TimeDistribution());
            Servers[ser].TimeDistribution[listOfTime].Time = t;
            Servers[ser].TimeDistribution[listOfTime].Probability = prob;
            ComProb += prob;
            Servers[ser].TimeDistribution[listOfTime].CummProbability = ComProb;
            Servers[ser].TimeDistribution[listOfTime].MinRange = min + 1;
            Servers[ser].TimeDistribution[listOfTime].MaxRange = Convert.ToInt32(ComProb * 100);
            min = Servers[ser].TimeDistribution[listOfTime].MaxRange;
            if (ComProb == 1)
            {
                ComProb = 0;
            }
            if (min == 100)
            {
                min = 0;
            }


        }
        public void intertime_dist_set(int t, decimal prob, int cust, int rows, int listOfTime)
        {
            InterarrivalDistribution.Add(new TimeDistribution());
            InterarrivalDistribution[listOfTime].Time = t;
            InterarrivalDistribution[listOfTime].Probability = prob;
            IntComProb += prob;
            InterarrivalDistribution[listOfTime].CummProbability = IntComProb;

            InterarrivalDistribution[listOfTime].MinRange = Imin + 1;
            InterarrivalDistribution[listOfTime].MaxRange = Convert.ToInt32(IntComProb * 100);
            Imin = InterarrivalDistribution[listOfTime].MaxRange;

        }
        //===========================================================================================================
        //===============RANDOM FUNCTION===============================================================================
        public int Randomize()
        {
            int x;
            x = r.Next(1, 100);
            return x;
        }
        //===================================================================================================================
        //===============================CASE HANDLER (FOR NUMBER OF CUS OR TIME)=========================================
        public void GenerateRand(SimulationSystem s)
        {
            int RINER, RSERVICE_T;
            //=================FOR CEREATION NUMBER OF CUSTOMERS==========================
            if (s.StoppingCriteria == Enums.StoppingCriteria.NumberOfCustomers)
            {
                for (int i = 0; i < s.StoppingNumber; i++)
                {
                    RINER = Randomize();
                    SimulationTable.Add(new SimulationCase());
                    SimulationTable[i].RandomInterArrival = RINER;
                    countofcustomer++;
                }
                SimulationTable[0].InterArrival = 0;
                for (int x = 1; x < countofcustomer; x++)
                {
                    for (int i = 0; i < InterarrivalDistribution.Count(); i++)
                    {
                        if (SimulationTable[x].RandomInterArrival >= InterarrivalDistribution[i].MinRange && SimulationTable[x].RandomInterArrival <= InterarrivalDistribution[i].MaxRange)
                        {
                            SimulationTable[x].InterArrival = InterarrivalDistribution[i].Time;
                            break;
                        }
                    }
                }
                for (int i = 0; i < countofcustomer; i++)
                {
                    RSERVICE_T = Randomize();
                    SimulationTable[i].RandomService = RSERVICE_T;
                }

            }
            //=========================FOR CEREATION TIME============================================
            else if (s.StoppingCriteria == Enums.StoppingCriteria.SimulationEndTime)
            {

                while (shrouk <= s.StoppingNumber)
                {
                    RINER = Randomize();
                    SimulationTable.Add(new SimulationCase());
                    SimulationTable[countofcustomer].RandomInterArrival = RINER;
                    for (int i = 0; i < InterarrivalDistribution.Count(); i++)
                    {
                        if (SimulationTable[countofcustomer].RandomInterArrival >= InterarrivalDistribution[i].MinRange && SimulationTable[countofcustomer].RandomInterArrival <= InterarrivalDistribution[i].MaxRange)
                        {
                            SimulationTable[countofcustomer].InterArrival = InterarrivalDistribution[i].Time;
                            shrouk += SimulationTable[countofcustomer].InterArrival;
                            break;
                        }
                    }
                    if (shrouk == s.StoppingNumber)
                    {
                        countofcustomer++;
                        break;

                    }
                    countofcustomer++;
                }
            }
            for (int i = 0; i < countofcustomer; i++)
            {
                RSERVICE_T = Randomize();
                SimulationTable[i].RandomService = RSERVICE_T;
            }


        }
        public void ArrivalTimeCalc(SimulationSystem s)
        {

            s.SimulationTable[0].ArrivalTime = 0;

            for (int i = 1; i < s.SimulationTable.Count(); i++)
            {
                s.SimulationTable[i].ArrivalTime = s.SimulationTable[i - 1].ArrivalTime + s.SimulationTable[i].InterArrival;


            }
        }
        //==================================RANDOOOOOM==========================================================================
        public void Random(int customer, SimulationSystem s)
        {
            List<int> FreeServers = new List<int>();
            int counter = 0;
            

            for (int i = 0; i < Servers.Count(); i++)
            {
                FreeServers.Add(new int());
            }

            for (int i = 0; i < Servers.Count(); i++)
            {
                if (Servers[i].FinishTime <= s.SimulationTable[customer - 1].ArrivalTime)
                {
                    Servers[i].IsIdle = true;
                    counter++;
                    FreeServers[i] = s.Servers[i].ID;
                }
            }

            for (int x = 0; x < Servers.Count(); x++)
            {

                if (s.Servers[x].IsIdle == true)
                {
                    if (counter > 1)
                    {
                        int y = 0;
                        y = r.Next(0, counter - 1);
                        x = FreeServers[y];
                        x--;
                        counter = 0;

                    }
                    s.SimulationTable[customer - 1].AssignedServer.ID = x + 1;
                    s.SimulationTable[customer - 1].AssignedServer.IsIdle = false;
                    s.Servers[x].IsIdle = false;
                    for (int j = 0; j < Servers[x].TimeDistribution.Count(); j++)
                    {
                        if (s.SimulationTable[customer - 1].RandomService <= Servers[x].TimeDistribution[j].MaxRange && s.SimulationTable[customer - 1].RandomService >= Servers[x].TimeDistribution[j].MinRange)
                        {
                            s.SimulationTable[customer - 1].ServiceTime = Servers[x].TimeDistribution[j].Time;
                            s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime;
                            s.SimulationTable[customer - 1].TimeInQueue = s.SimulationTable[customer - 1].StartTime - s.SimulationTable[customer - 1].ArrivalTime;
                            s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime + s.SimulationTable[customer - 1].TimeInQueue;
                            s.SimulationTable[customer - 1].EndTime = s.SimulationTable[customer - 1].ServiceTime + s.SimulationTable[customer - 1].StartTime;
                            Servers[x].FinishTime = s.SimulationTable[customer - 1].EndTime;
                            counterToAddInQueue = 0;
                            break;
                        }

                    }
                    if (counterToAddInQueue == 0)
                    {
                        break;
                    }
                }
                counterToAddInQueue++;
            }
            if (counterToAddInQueue == s.Servers.Count())
            {
                int counterr = -1;
                int jj = 0;
                int min = 9999999;
                int id = 0;
                int index = 0;
                for (int i = 0; i < Servers.Count(); i++)
                {
                    int z = Servers[i].FinishTime;

                    if (z < min)
                    {
                        min = z;
                        id = i + 1;
                        index = i;
                    }

                }
                FreeServers[jj] = id;
                jj++;
                for (int i = 0; i < Servers.Count(); i++)
                {
                    if (min == Servers[i].FinishTime)
                    {
                        counterr++;
                    }
                    if (counterr > 0)
                    {
                        FreeServers[jj] = i + 1;
                        jj++;
                    }
                }
                id = FreeServers[r.Next(0, jj - 1)];
                s.SimulationTable[customer - 1].AssignedServer.ID = id;
                for (int j = 0; j < Servers[id - 1].TimeDistribution.Count(); j++)
                {
                    if (s.SimulationTable[customer - 1].RandomService <= Servers[id - 1].TimeDistribution[j].MaxRange && s.SimulationTable[customer - 1].RandomService >= Servers[id - 1].TimeDistribution[j].MinRange)
                    {
                        s.SimulationTable[customer - 1].ServiceTime = Servers[id - 1].TimeDistribution[j].Time;
                        s.SimulationTable[customer - 1].StartTime = min;
                        s.SimulationTable[customer - 1].TimeInQueue = min - s.SimulationTable[customer - 1].ArrivalTime;
                        if (s.SimulationTable[customer - 1].TimeInQueue < 0)
                        {
                            s.SimulationTable[customer - 1].TimeInQueue = 0;
                        }
                        s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime + s.SimulationTable[customer - 1].TimeInQueue;
                        if (min > s.SimulationTable[customer - 1].ArrivalTime)
                        {
                            s.SimulationTable[customer - 1].StartTime = min;
                        }

                        s.SimulationTable[customer - 1].EndTime = s.SimulationTable[customer - 1].ServiceTime + s.SimulationTable[customer - 1].StartTime;
                        Servers[id - 1].FinishTime = s.SimulationTable[customer - 1].EndTime;
                        counterToAddInQueue = 0;
                        break;
                    }

                }

            }
        }
        //==================================PIRIORITY=====================================================================================
        public void piriority(int customer, SimulationSystem s)
        {
            for (int i = 0; i < Servers.Count(); i++)
            {
                if (Servers[i].FinishTime <= s.SimulationTable[customer - 1].ArrivalTime)
                {
                    Servers[i].IsIdle = true;
                }
            }

            for (int x = 0; x < Servers.Count(); x++)
            {

                if (s.Servers[x].IsIdle == true)
                {

                    s.SimulationTable[customer - 1].AssignedServer.ID = x + 1;
                    s.SimulationTable[customer - 1].AssignedServer.IsIdle = false;
                    s.Servers[x].IsIdle = false;
                    for (int j = 0; j < Servers[x].TimeDistribution.Count(); j++)
                    {
                        if (s.SimulationTable[customer - 1].RandomService <= Servers[x].TimeDistribution[j].MaxRange && s.SimulationTable[customer - 1].RandomService >= Servers[x].TimeDistribution[j].MinRange)
                        {
                            s.SimulationTable[customer - 1].ServiceTime = Servers[x].TimeDistribution[j].Time;
                            s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime;
                            s.SimulationTable[customer - 1].TimeInQueue = s.SimulationTable[customer - 1].StartTime - s.SimulationTable[customer - 1].ArrivalTime;
                            s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime + s.SimulationTable[customer - 1].TimeInQueue;
                            s.SimulationTable[customer - 1].EndTime = s.SimulationTable[customer - 1].ServiceTime + s.SimulationTable[customer - 1].StartTime;
                            Servers[x].FinishTime = s.SimulationTable[customer - 1].EndTime;
                            counterToAddInQueue = 0;
                            break;
                        }

                    }
                    if (counterToAddInQueue == 0)
                    {
                        break;
                    }
                }
                counterToAddInQueue++;
            }
            if (counterToAddInQueue == Servers.Count())
            {

                int min = 9999999;
                int id = 0;
                int index = 0;
                for (int i = 0; i < Servers.Count(); i++)
                {
                    int z = Servers[i].FinishTime;

                    if (z < min)
                    {
                        min = z;
                        id = i + 1;
                        index = i;
                    }
                    s.SimulationTable[customer - 1].AssignedServer.ID = id;

                }


                for (int j = 0; j < Servers[id - 1].TimeDistribution.Count(); j++)
                {
                    if (s.SimulationTable[customer - 1].RandomService <= Servers[id - 1].TimeDistribution[j].MaxRange && s.SimulationTable[customer - 1].RandomService >= Servers[id - 1].TimeDistribution[j].MinRange)
                    {
                        s.SimulationTable[customer - 1].ServiceTime = Servers[id - 1].TimeDistribution[j].Time;
                        s.SimulationTable[customer - 1].StartTime = min;
                        s.SimulationTable[customer - 1].TimeInQueue = min - s.SimulationTable[customer - 1].ArrivalTime;
                        if (s.SimulationTable[customer - 1].TimeInQueue < 0)
                        {
                            s.SimulationTable[customer - 1].TimeInQueue = 0;
                        }
                        s.SimulationTable[customer - 1].StartTime = s.SimulationTable[customer - 1].ArrivalTime + s.SimulationTable[customer - 1].TimeInQueue;
                        if (min > s.SimulationTable[customer - 1].ArrivalTime)
                        {
                            s.SimulationTable[customer - 1].StartTime = min;
                        }

                        s.SimulationTable[customer - 1].EndTime = s.SimulationTable[customer - 1].ServiceTime + s.SimulationTable[customer - 1].StartTime;
                        Servers[id - 1].FinishTime = s.SimulationTable[customer - 1].EndTime;
                        counterToAddInQueue = 0;
                        break;
                    }

                }

            }
        }
        public decimal average_waiting_time(SimulationSystem s)
        {
            decimal totalInQueue = 0;
            for (int i = 0; i < s.SimulationTable.Count(); i++)
            {
                totalInQueue += s.SimulationTable[i].TimeInQueue;
            }
            if (s.SimulationTable.Count() != 0)
            {
                s.PerformanceMeasures.AverageWaitingTime = totalInQueue / s.SimulationTable.Count();
            }

            return s.PerformanceMeasures.AverageWaitingTime;
        }
        public decimal probOfWaitInSystem(SimulationSystem s)
        {
            decimal counter = 0;
            for (int i = 0; i < s.SimulationTable.Count(); i++)
            {
                if (s.SimulationTable[i].TimeInQueue > 0)
                {
                    counter++;
                }
            }
            if (s.SimulationTable.Count() != 0)
            {
                s.PerformanceMeasures.WaitingProbability = counter / s.SimulationTable.Count();
            }

            return s.PerformanceMeasures.WaitingProbability;
        }
        public int mxQueueLength(SimulationSystem s)
        {
            SimulationSystem ss = new SimulationSystem();
            int length = 0;
            int max = 0;

            for (int i = 0; i < s.SimulationTable.Count(); i++)
            {
                if (s.SimulationTable[i].TimeInQueue > 0)
                {
                    QSC.Enqueue(s.SimulationTable[i]);
                }
            }
            if (QSC.Count() > 0)
            {
                max = 1;
            }
            for (int x = 0; x < QSC.Count(); x++)
            {
                ss.SimulationTable.Add(new SimulationCase());

            }
            for (int z = 0; z < ss.SimulationTable.Count(); z++)
            {

                ss.SimulationTable[z] = QSC.Dequeue();

            }
            for (int q = 0; q < ss.SimulationTable.Count(); q++)
            {
                int counter = 0;
                if (q == 0)
                {
                    length++;
                }
                else
                {
                    for (int j = q - 1; j >= 0; j--)
                    {

                        if (ss.SimulationTable[q].ArrivalTime < ss.SimulationTable[j].StartTime)
                        {
                            counter++;
                        }
                    }
                    if (counter >= length)
                    {
                        length++;
                        if (length > max)
                        {
                            max = length;
                        }
                    }
                }

            }
            s.PerformanceMeasures.MaxQueueLength = max;
            return max;
        }
        public void Graph_Handler(int ID, SimulationSystem s)
        {
            ssss = new SimulationSystem();
            int j = 0;
            for (int i = 0; i < s.SimulationTable.Count(); i++)
            {
                if (s.SimulationTable[i].AssignedServer.ID == ID)
                {
                    ssss.SimulationTable.Add(new SimulationCase());
                    ssss.SimulationTable[j] = s.SimulationTable[i];
                    j++;
                }
            }
        }
        public int Graph_Draw_Start(int i)
        {
            int start;
            start = ssss.SimulationTable[i].StartTime;
            return start;
        }
        public int Graph_Draw_end(int i)
        {
            int end;
            end = ssss.SimulationTable[i].EndTime;
            return end;
        }
        public int graphOfZeroStart(int i)
        {
            int start = 0;
            if (i > 0)
            {
                if (ssss.SimulationTable[i].StartTime - ssss.SimulationTable[i - 1].EndTime > 0)
                {
                    start = ssss.SimulationTable[i - 1].EndTime;
                }
            }
            return start;
        }
        public int graphOfZeroEnd(int i)
        {
            int end = 0;
            if (i > 0)
            {
                if (ssss.SimulationTable[i].StartTime - ssss.SimulationTable[i - 1].EndTime > 0)
                {
                    end = ssss.SimulationTable[i].StartTime;
                }
            }
            return end;
        }
        public decimal serveridlePROB(int ID, SimulationSystem s)
        {
            sepcserver = new SimulationSystem();
            decimal prob = 0;
            decimal total = s.SimulationTable[s.SimulationTable.Count() - 1].EndTime;

            for (int q = 0; q < s.SimulationTable.Count(); q++)
            {

                if (ID == s.SimulationTable[q].AssignedServer.ID)
                {
                    sepcserver.SimulationTable.Add(new SimulationCase());
                    sepcserver.SimulationTable[T] = s.SimulationTable[q];
                    T++;
                }
            }

            int last = sepcserver.SimulationTable.Count();
            for (int i = 1; i < sepcserver.SimulationTable.Count(); i++)
            {
                prob += sepcserver.SimulationTable[i].StartTime - sepcserver.SimulationTable[i - 1].EndTime;

            }
            if (last == 0)
            {
                prob = total;
            }
            else
            {
                prob += sepcserver.SimulationTable[0].StartTime - s.SimulationTable[0].StartTime;
                prob += total - sepcserver.SimulationTable[last - 1].EndTime;
            }

            if (total != 0)
            {

                s.Servers[ID - 1].IdleProbability = prob / total;
            }
            
            return s.Servers[ID - 1].IdleProbability;

        }
        public decimal serverAVGtime(int ID, SimulationSystem s)
        {
            decimal avg = 0;
            decimal totalcust = sepcserver.SimulationTable.Count();
            for (int i = 0; i < sepcserver.SimulationTable.Count(); i++)
            {
                avg += sepcserver.SimulationTable[i].EndTime - sepcserver.SimulationTable[i].StartTime;

            }
            if (totalcust != 0)
            {

                s.Servers[ID - 1].AverageServiceTime = avg / totalcust;
            }

            return s.Servers[ID - 1].AverageServiceTime;

        }
        public decimal Utilization(int ID, SimulationSystem s)
        {
            decimal avg = 0;
            decimal total = s.SimulationTable[s.SimulationTable.Count() - 1].EndTime;
            for (int i = 0; i < sepcserver.SimulationTable.Count(); i++)
            {
                avg += sepcserver.SimulationTable[i].EndTime - sepcserver.SimulationTable[i].StartTime;

            }
            if (total != 0)
            {

                s.Servers[ID - 1].Utilization = avg / total;
            }
            T = 0;
            return s.Servers[ID - 1].Utilization;

        }

    }
}
