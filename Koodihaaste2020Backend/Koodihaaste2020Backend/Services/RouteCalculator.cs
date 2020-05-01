using Koodihaaste2020Backend.Interfaces;
using Koodihaaste2020Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Koodihaaste2020Backend.Services
{
    /// <summary>
    /// Used to calculate the fastest route between two bus stops.
    /// </summary>
    public class RouteCalculator : IRouteCalculator
    {
        private BusDataRespository dataRepository;

        public RouteCalculator()
        {
            dataRepository = new BusDataRespository();
        }

        /// <summary>
        /// Used to calculate the fastest route between two bus stops.
        /// The algorithm works in following fashion:
        ///     1) Get possible routes from the stop being processed
        ///     2) Find out which lines use those routes
        ///     3) Recursively do the steps 1) & 2) until all routes with lines are found
        ///     4) Find out shortest of the routes in the recursive calls
        ///     5) Sort the route bus stops to be easily processable in the UI
        /// </summary>
        public List<BusLineRoute> CalculateRoute(string fromStopName, string toStopName)
        {
            GetCalculationBusData(ref fromStopName, ref toStopName, out var allRoutes, out var allLines);

            var possibleRoutes = allRoutes.Where(route => route.BusStop1.Name.Equals(fromStopName)
                                                            || route.BusStop2.Name.Equals(fromStopName));
            var allLinesRoutes = GetPossibleLineRoutes(allLines, possibleRoutes);

            var finalRoutes = CalculatePossibleRoutes(true, allRoutes, allLines, null, fromStopName, fromStopName, toStopName, allLinesRoutes);
            var finalLineRoute = DetermineShortestRoute(finalRoutes);
            return OrderLineRoute(finalLineRoute, fromStopName);
        }

        /// <summary>
        /// BusRoute BusStops can be in any order. Order them so that they are easy to display on the client.
        /// </summary>
        private List<BusLineRoute> OrderLineRoute(List<BusLineRoute> finalLineRoute, string fromStopName)
        {
            var orderedList = new List<BusLineRoute>();
            foreach (var lineRoute in finalLineRoute)
            {
                if(orderedList.Count == 0) // First route
                {
                    if(lineRoute.Route.BusStop1.Name.Equals(fromStopName))
                    {
                        orderedList.Add(lineRoute);
                        continue; // They are in correct order already - BusStop1 is the start stop
                    } else
                    {
                        // Change the bus stops around
                        var busStop1 = lineRoute.Route.BusStop1;
                        var busStop2 = lineRoute.Route.BusStop2;
                        lineRoute.Route.BusStop1 = busStop2;
                        lineRoute.Route.BusStop2 = busStop1;
                        orderedList.Add(lineRoute);
                    }
                } else
                {
                    var lastLineRoute = orderedList.Last(); // Get the last one that has been ordered
                    var lastBusStop = lastLineRoute.Route.BusStop2; // Check where that route ended in
                    if (lastLineRoute.Color.Name.Equals(lineRoute.Color.Name)) // Same color --> change the previous route's BusStop2 and update duration
                    {
                        var busStopThatReplacesPreviousBusStop2 = lineRoute.Route.BusStop1;
                        if (!lineRoute.Route.BusStop2.Name.Equals(lastBusStop.Name))
                        {
                            busStopThatReplacesPreviousBusStop2 = lineRoute.Route.BusStop2;
                        }
                        lastLineRoute.Route.BusStop2 = busStopThatReplacesPreviousBusStop2;
                        lastLineRoute.Route.Duration += lineRoute.Route.Duration;
                        orderedList.RemoveAt(orderedList.Count - 1);
                        orderedList.Add(lastLineRoute);
                    } else // Last lines are different colors --> add the lineRoute to the orderedList
                    {
                        if (lineRoute.Route.BusStop1.Name.Equals(lastBusStop.Name))
                        {
                            orderedList.Add(lineRoute);
                            continue; // They are in correct order already - BusStop1 is the start stop
                        }
                        else
                        {
                            // Change the bus stops around
                            var busStop1 = lineRoute.Route.BusStop1;
                            var busStop2 = lineRoute.Route.BusStop2;
                            lineRoute.Route.BusStop1 = busStop2;
                            lineRoute.Route.BusStop2 = busStop1;
                            orderedList.Add(lineRoute);
                        }
                    }
                }
            }
            return orderedList;
        }

        private List<BusLineRoute> CalculateNextRoute(List<BusRoute> allRoutes, List<BusLine> allLines, List<BusLineRoute> travelledRoutes, string originalFromStopName,
                                                string currentFromStopName, string toStopName)
        {
            // Let's pick those routes that 1) contain the current stop 2) don't end in the beginning 3) haven't been travelled
            var possibleRoutes = allRoutes.Where(route => (
                                                                (route.BusStop1.Name.Equals(currentFromStopName) && !route.BusStop2.Name.Equals(originalFromStopName))
                                                                ||
                                                                (route.BusStop2.Name.Equals(currentFromStopName) && !route.BusStop1.Name.Equals(originalFromStopName))
                                                            ) && !travelledRoutes.Any(lineRoute => lineRoute.Route.BusStop1.Name.Equals(route.BusStop1.Name)
                                                                                                    && lineRoute.Route.BusStop2.Name.Equals(route.BusStop2.Name))
                                                );

            if (possibleRoutes == null || possibleRoutes.Count() == 0)
            {
                return null;
            }

            var allLinesRoutes = GetPossibleLineRoutes(allLines, possibleRoutes);

            var possibleRouteTravelledLists = CalculatePossibleRoutes(false, allRoutes, allLines, travelledRoutes, originalFromStopName, currentFromStopName, toStopName, allLinesRoutes);
            return DetermineShortestRoute(possibleRouteTravelledLists);
        }

        private List<List<BusLineRoute>> CalculatePossibleRoutes(bool firstCall, List<BusRoute> allRoutes, List<BusLine> allLines,
                                        List<BusLineRoute> travelledRoutes, string originalFromStopName, string currentFromStopName, 
                                        string toStopName, List<BusLineRoute> allLinesRoutes)
        {
            var possibleRouteTravelledLists = new List<List<BusLineRoute>>();
            foreach (var lineRoute in allLinesRoutes)
            {
                var travelledRoutesAtThisPoint = new List<BusLineRoute>();
                if (!firstCall)
                {
                    travelledRoutesAtThisPoint.AddRange(travelledRoutes);
                }
                travelledRoutesAtThisPoint.Add(lineRoute);


                if ((lineRoute.Route.BusStop1.Name.Equals(currentFromStopName) && lineRoute.Route.BusStop2.Name.Equals(toStopName))
                        || (lineRoute.Route.BusStop2.Name.Equals(currentFromStopName) && lineRoute.Route.BusStop1.Name.Equals(toStopName)))
                {
                    possibleRouteTravelledLists.Add(travelledRoutesAtThisPoint);
                }
                else
                {
                    var nextStopName = lineRoute.Route.BusStop1.Name;
                    if (lineRoute.Route.BusStop1.Name.Equals(currentFromStopName)) { nextStopName = lineRoute.Route.BusStop2.Name; }

                    var list = CalculateNextRoute(allRoutes, allLines, travelledRoutesAtThisPoint, originalFromStopName, nextStopName, toStopName);
                    if (list != null && list.Count() > 0)
                        possibleRouteTravelledLists.Add(list);
                }
            }

            return possibleRouteTravelledLists;
        }

        private void GetCalculationBusData(ref string fromStopName, ref string toStopName, out List<BusRoute> allRoutes, out List<BusLine> allLines)
        {
            fromStopName = fromStopName.ToUpper();
            toStopName = toStopName.ToUpper();
            allRoutes = dataRepository.GetBusRoutes();
            allLines = dataRepository.GetBusLines();
        }

        private static List<BusLineRoute> GetPossibleLineRoutes(List<BusLine> allLines, IEnumerable<BusRoute> possibleRoutes)
        {
            var allLinesRoutes = new List<BusLineRoute>();
            foreach (var line in allLines)
            {
                foreach (var route in line.Routes)
                {
                    if (possibleRoutes.Any(pRoute => route.BusStop1.Name.Equals(pRoute.BusStop1.Name)
                                                && route.BusStop2.Name.Equals(pRoute.BusStop2.Name)))
                        allLinesRoutes.Add(new BusLineRoute(line.Color, route));
                }
            };
            return allLinesRoutes;
        }

        private static List<BusLineRoute> DetermineShortestRoute(List<List<BusLineRoute>> possibleRouteTravelledLists)
        {
            var leadingRoute = Tuple.Create(-1, new List<BusLineRoute>());
            foreach (var travelledLineRoute in possibleRouteTravelledLists)
            {
                var sum = travelledLineRoute.Sum(lineRoute => lineRoute.Route.Duration);
                if(sum == leadingRoute.Item1) // If the length of the leading route is same as the one being compared to, choose the one with less lines used
                {
                    Dictionary<string, int> currentDict = GetAmountOfTravelledColorDict(travelledLineRoute);
                    Dictionary<string, int> leadingDict = GetAmountOfTravelledColorDict(leadingRoute.Item2);
                    if (currentDict.Keys.Count() < leadingDict.Count())
                    {
                        leadingRoute = Tuple.Create(sum, travelledLineRoute);
                        continue;
                    }
                }
                if (leadingRoute.Item1 == -1 || sum < leadingRoute.Item1)
                    leadingRoute = Tuple.Create(sum, travelledLineRoute);
            }

            return leadingRoute.Item2;
        }

        private static Dictionary<string, int> GetAmountOfTravelledColorDict(List<BusLineRoute> travelledLineRoute)
        {
            var currentDict = new Dictionary<string, int>();
            foreach (var lineRoute in travelledLineRoute)
            {
                if (currentDict.ContainsKey(lineRoute.Color.Name))
                {
                    currentDict[lineRoute.Color.Name] = currentDict[lineRoute.Color.Name] + 1;
                }
                else
                {
                    currentDict[lineRoute.Color.Name] = 1;
                }
            }

            return currentDict;
        }
    }
}