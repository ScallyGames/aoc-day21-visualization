using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C5;
using DefaultNamespace;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Visualizer : MonoBehaviour
{
    const ulong maxNumberOfUniverses = 570239341223618UL;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float scale = 10;
    public Gradient gradient;
    
    private int numberOfMovingElements = 0;

    private void Start()
    {
        StartCoroutine(DoCalculation());
    }

    IEnumerator DoCalculation()
    {
        yield return new WaitForEndOfFrame();
        IDie die = new DiracDie();

        GameState initialGameState = new GameState(new PlayerState(7, 0), new PlayerState(4, 0), 0);


        Dictionary<GameState, ulong> gameStateOccurrences = new Dictionary<GameState, ulong>();
        Dictionary<GameState, GameObject> gameStateObjects = new Dictionary<GameState, GameObject>();
        IPriorityQueue<GameState> openGameStates = new IntervalHeap<GameState>(Comparer<GameState>.Create((a, b) => a.MaxScore - b.MaxScore));

        gameStateOccurrences.Add(initialGameState, 1);
        openGameStates.Add(initialGameState);

        while(openGameStates.Count > 0)
        {
            // while (numberOfMovingElements > 0) yield return new WaitForEndOfFrame(); 
            
            var gameStateToHandle = openGameStates.DeleteMin();
            if (!gameStateOccurrences.ContainsKey(gameStateToHandle)) continue;

            // Console.WriteLine(gameStateToHandle.GetHashCode() + " splits into:");

            List<RollResult> rollResults = new List<RollResult>();
            rollResults.Add(new RollResult(0, 1));
            
            for(int i = 0; i < GameSettings.NumberOfRolls; i++)
            {
                var dieResult = die.GetRollResult(); 
                rollResults = rollResults
                    .SelectMany<RollResult, int, RollResult>(x => dieResult, (a, b) => new RollResult(a.Roll + b, a.NumberOfTimesRolled))
                    .GroupBy(x => x.Roll)
                    .Select(x => new RollResult(x.Key, (byte)x.Sum(x => x.NumberOfTimesRolled)))
                    .ToList();
            }

            foreach(var roll in rollResults)
            {
                GameState newGameState;
                if(gameStateToHandle.PlayerOnTurn == 0)
                {
                    int newPosition = gameStateToHandle.Player0State.CurrentPosition + roll.Roll;
                    newPosition = ((newPosition - 1) % GameSettings.NumberOfFieldsInCircle) + 1;
                    int newScore = gameStateToHandle.Player0State.CurrentScore + newPosition;
                    PlayerState newPlayerState = new PlayerState(newPosition, newScore);
                    newGameState = new GameState(newPlayerState, gameStateToHandle.Player1State, (gameStateToHandle.PlayerOnTurn + 1) % 2);
                }
                else
                {
                    int newPosition = gameStateToHandle.Player1State.CurrentPosition + roll.Roll;
                    newPosition = ((newPosition - 1) % GameSettings.NumberOfFieldsInCircle) + 1;
                    int newScore = gameStateToHandle.Player1State.CurrentScore + newPosition;
                    PlayerState newPlayerState = new PlayerState(newPosition, newScore);
                    newGameState = new GameState(gameStateToHandle.Player0State, newPlayerState, (gameStateToHandle.PlayerOnTurn + 1) % 2);
                }
                
                // Console.Write($"    {newGameState.GetHashCode()}");
                if(!gameStateOccurrences.ContainsKey(newGameState))
                {
                    // Console.Write(" which has not yet been reached");
                    gameStateOccurrences.Add(newGameState, 0);

                    numberOfMovingElements++;
                    var newObject = GameObject.Instantiate(prefab);
                    if (!gameStateObjects.ContainsKey(newGameState))
                    {
                        gameStateObjects.Add(newGameState, newObject);
                    }
                    newObject.transform.position = Vector3.right * gameStateToHandle.Player0State.CurrentScore * this.scale + Vector3.forward * gameStateToHandle.Player1State.CurrentScore * this.scale;
                    TrailRenderer trailRenderer = newObject.GetComponent<TrailRenderer>();
                    var t = ((double) gameStateOccurrences[gameStateToHandle] / (double) maxNumberOfUniverses);
                    var tEased = (float) ((20 + Math.Log(t)) / (double) 20);
                    Debug.Log(tEased);
                    trailRenderer.material.color = gradient.Evaluate(tEased);
                    
                    MoveToTarget moveToTarget = newObject.GetComponent<MoveToTarget>();
                    moveToTarget.TargetPosition = Vector3.right * newGameState.Player0State.CurrentScore * this.scale + Vector3.forward * newGameState.Player1State.CurrentScore * this.scale;
                    moveToTarget.ReachedTarget += (sender, args) =>
                    {
                        numberOfMovingElements--;
                        if (!gameStateObjects.ContainsKey(newGameState))
                        {
                            gameStateObjects.Add(newGameState, newObject);
                        }
                        else
                        {
                            if (gameStateObjects[newGameState] != newObject)
                            {
                                foreach (Transform child in newObject.transform)
                                {
                                    GameObject.Destroy(child.gameObject);
                                }
                            }
                        }
                    };

                    if(!newGameState.IsCompleted)
                    {
                        openGameStates.Add(newGameState);
                    }
                }
                else
                {
                    // Console.Write($" which has already been reached {gameStateOccurances[newGameState]} times");
                }
                gameStateOccurrences[newGameState] += gameStateOccurrences[gameStateToHandle] * roll.NumberOfTimesRolled;
                
                // Console.Write($" and has now been reached {gameStateOccurances[newGameState]} times");
                // Console.WriteLine();
            }

            if (gameStateObjects.ContainsKey(gameStateToHandle))
            {
                foreach (Transform child in gameStateObjects[gameStateToHandle].transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            gameStateOccurrences.Remove(gameStateToHandle);
        }
        
        
        ulong numberOfGamesWonByPlayer0 = gameStateOccurrences.Where(x => x.Key.Player0State.HasWon).Aggregate(0UL, (a, b) => a + b.Value);
        ulong numberOfGamesWonByPlayer1 = gameStateOccurrences.Where(x => x.Key.Player1State.HasWon).Aggregate(0UL, (a, b) => a + b.Value);


        UnityEngine.Debug.Log(numberOfGamesWonByPlayer0);
        UnityEngine.Debug.Log(numberOfGamesWonByPlayer1);
    }

    void Update()
    {
        
    }
}
