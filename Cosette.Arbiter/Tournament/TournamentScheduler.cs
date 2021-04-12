using System.Collections.Generic;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentScheduler
    {
        private int _participantsCount;
        private List<(int playerA, int playerB)> _pairs;

        public void Init(int participantsCount, bool gauntlet)
        {
            _participantsCount = participantsCount;
            _pairs = new List<(int playerA, int playerB)>();

            if (gauntlet)
            {
                for (var gameNumber = 1; gameNumber < _participantsCount; gameNumber++)
                {
                    _pairs.Add((0, gameNumber));
                }
            }
            else
            {
                if (_participantsCount % 2 != 0)
                {
                    _participantsCount++;
                }

                for (var gameNumber = 0; gameNumber < _participantsCount - 1; gameNumber++)
                {
                    var firstRow = new List<int>();
                    var secondRow = new List<int>();
                    var teamsCount = _participantsCount / 2;

                    firstRow.Add(1);

                    var secondParticipant = _participantsCount - gameNumber;
                    var currentParticipant = secondParticipant;

                    for (var i = 1; i < teamsCount; i++)
                    {
                        currentParticipant++;
                        if (currentParticipant > _participantsCount)
                        {
                            currentParticipant = 2;
                        }

                        firstRow.Add(currentParticipant);
                    }

                    for (var i = 0; i < teamsCount; i++)
                    {
                        currentParticipant++;
                        if (currentParticipant > _participantsCount)
                        {
                            currentParticipant = 2;
                        }

                        secondRow.Add(currentParticipant);
                    }

                    secondRow.Reverse();

                    for (var i = 0; i < teamsCount; i++)
                    {
                        _pairs.Add((firstRow[i] - 1, secondRow[i] - 1));
                    }
                }
            }
        }

        public (int playerA, int playerB) GetPair(int gameNumber)
        {
            var pairIndex = gameNumber % _pairs.Count;
            return _pairs[pairIndex];
        }
    }
}
