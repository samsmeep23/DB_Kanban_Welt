using System.Collections.Generic;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    [SerializeField] private Transform tasksColumn;        // Parent transform for "Tasks" column
    [SerializeField] private Transform arbeitstand1Column; // Parent transform for "Arbeitstand 1"
    [SerializeField] private Transform arbeitstand2Column; // Parent transform for "Arbeitstand 2"
    [SerializeField] private Transform arbeitstand3Column; // Parent transform for "Arbeitstand 3"

    [SerializeField] private MemoryGame memoryGame;

    private List<Transform> tasks = new List<Transform>();
    private int arbeitstand1Count = 0;
    private int arbeitstand2Count = 0;
    private int arbeitstand3Count = 0;

    private float taskSpacing = 1.0f; // Initialize with default, updated dynamically

    public int Arbeitstand3TaskCount => arbeitstand3Count;
    private void Start()
    {
        tasks = new List<Transform>();

        // Calculate initial spacing dynamically based on Tasks column
        if (tasksColumn.childCount > 1)
        {
            taskSpacing = Mathf.Abs(tasksColumn.GetChild(1).localPosition.y - tasksColumn.GetChild(0).localPosition.y);
        }

        // Collect all tasks initially in the "Tasks" column
        foreach (Transform task in tasksColumn)
        {
            tasks.Add(task);
        }
    }

    public void OnCardsMatched(int arbeitstandType)
    {
        if (arbeitstandType == 1)
        {
            MoveTaskToArbeitstand1();
        }
        else if (arbeitstandType == 2)
        {
            MoveTasksToArbeitstand2();
        }
        else if (arbeitstandType == 3)
        {
            MoveTasksToArbeitstand3();
        }
    }

    private void MoveTaskToArbeitstand1()
    {
        if (arbeitstand1Count < tasks.Count)
        {
            Transform task = tasks[arbeitstand1Count];
            task.SetParent(arbeitstand1Column);
            task.localPosition = new Vector3(0, -arbeitstand1Count * taskSpacing, 0); // Use dynamic spacing
            arbeitstand1Count++;
        }
    }

    private void MoveTasksToArbeitstand2()
    {
        if (arbeitstand2Count < arbeitstand1Count)
        {
            Transform task = tasks[arbeitstand2Count];
            task.SetParent(arbeitstand2Column);
            task.localPosition = new Vector3(0, -arbeitstand2Count * taskSpacing, 0); // Use dynamic spacing
            arbeitstand2Count++;
        }
    }

    private void MoveTasksToArbeitstand3()
    {
        if (arbeitstand3Count < arbeitstand2Count)
        {
            Transform task = tasks[arbeitstand3Count];
            task.SetParent(arbeitstand3Column);
            task.localPosition = new Vector3(0, -arbeitstand3Count * taskSpacing, 0);
            arbeitstand3Count++;

            if (arbeitstand3Count == 3)
            {
                memoryGame.OnGameWon(); // Notify MemoryGame that the player has won
            }
        }
    }

    /* public bool ColumnHasTasks(int arbeitstandType)
     {
         if (arbeitstandType == 1) return arbeitstand1Count > 0;
         if (arbeitstandType == 2) return arbeitstand2Count > 0;
         if (arbeitstandType == 3) return arbeitstand3Count > 0;
         return false;
     }*/

     
     public void ResetToInitialTasks()
     {
        arbeitstand1Count = 0;
        arbeitstand2Count = 0;
        arbeitstand3Count = 0;

        for (int i = 0; i < tasks.Count; i++)
        {
            Transform task = tasks[i];
            task.SetParent(tasksColumn);
            task.localPosition = new Vector3(0, -i * taskSpacing, 0); // Reset positions based on index
        }
    }
    
    public bool PrecedingColumnHasTasks(int arbeitstandType)
    {
        if (arbeitstandType == 2)
        {
            // Check if Arbeitstand 1 has tasks
            return arbeitstand1Count > 0;
        }
        else if (arbeitstandType == 3)
        {
            // Check if Arbeitstand 2 has tasks
            return arbeitstand2Count > 0;
        }
        return false; // For Arbeitstand 1 or invalid input
    }



}
