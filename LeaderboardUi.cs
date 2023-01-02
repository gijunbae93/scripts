using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class LeaderboardUi : MonoBehaviour
{
    [SerializeField] Transform rankContainer;
    Transform rankUi;

    [SerializeField] Transform nameContainer;
    Transform nameUi;

    [SerializeField] Transform roundContainer;
    Transform roundUi;

    [SerializeField] Transform enemiesDefeatedContainer;
    Transform enemiesDefeatedUi;

    [SerializeField] Transform survivedContainer;
    Transform survivedUi;

    [SerializeField] Button nextButton;

    public static readonly int topSixPlayer = 6;

    CanvasGroup leaderboardCanvasGroup;

    TextMeshProUGUI playerRankInfo;
    private void Awake()
    {
        rankUi = rankContainer.Find("Text (TMP)");
        nameUi = nameContainer.Find("Text (TMP)");
        roundUi = roundContainer.Find("Text (TMP)");
        enemiesDefeatedUi = enemiesDefeatedContainer.Find("Text (TMP)");
        survivedUi = survivedContainer.Find("Text (TMP)");

        rankUi.gameObject.SetActive(false);
        nameUi.gameObject.SetActive(false);
        roundUi.gameObject.SetActive(false);
        enemiesDefeatedUi.gameObject.SetActive(false);
        survivedUi.gameObject.SetActive(false);

        leaderboardCanvasGroup = GetComponent<CanvasGroup>();
        SetLeaderboardUiActive(false);

        nextButton.onClick.AddListener(() => NextScene.OnNextScene());

        playerRankInfo = transform.Find("PlayerRankInfo (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateLeaderBoard(List<LeaderboardData> topPlayerLeaderboardList, List<LeaderboardData> allLeaderboardList, LeaderboardData playerLeaderboard)
    {
        int count = topPlayerLeaderboardList?.Count ?? 0;
        if(count == 0)
        {
            // not connected to internet
            return;
        }

        int y = 0; // only changing Y;
        int spaceBetweenUi = 40;
        Func<Transform, Transform, TextMeshProUGUI> OnDisplayingLeaderBoard = (t1, v1) =>
        {
            RectTransform recTransform = Instantiate(t1, v1).GetComponent<RectTransform>();
            recTransform.gameObject.SetActive(true);
            recTransform.anchoredPosition = new Vector2(0, -y * spaceBetweenUi); // negative sign front of y

            return recTransform.GetComponent<TextMeshProUGUI>();
        };
        foreach (LeaderboardData leaderBoardData in topPlayerLeaderboardList)
        {
            OnDisplayingLeaderBoard(rankUi, rankContainer).text = leaderBoardData.rank.ToString();
            Debug.Log(leaderBoardData.userName.ToString());
            OnDisplayingLeaderBoard(nameUi, nameContainer).text = leaderBoardData.userName.ToString();
            OnDisplayingLeaderBoard(roundUi, roundContainer).text = leaderBoardData.score.ToString();

            OnDisplayingLeaderBoard(enemiesDefeatedUi, enemiesDefeatedContainer).text = new FloatToAbbreviation().ConvertedString(leaderBoardData.defeatedEnemyNumberAndSurviveTimer[0]);

            OnDisplayingLeaderBoard(survivedUi, survivedContainer).text = new FloatToTimeConversion(leaderBoardData.defeatedEnemyNumberAndSurviveTimer[1]).ConversionValue();

            y++;
            if (y == topSixPlayer)
                break;
        }


        allLeaderboardList.Add(playerLeaderboard);

        List<LeaderboardData> finalLeaderboardList = new List<LeaderboardData>();
        int currentAllLeaderboardListCount = allLeaderboardList.Count;

        while(currentAllLeaderboardListCount != finalLeaderboardList.Count)
        {
            List<LeaderboardData> tempList = SortLeaderBoardSingleListCompletedRound(allLeaderboardList);
            if(tempList.Count == 1)
            {
                finalLeaderboardList.Add(tempList[0]);
                allLeaderboardList.Remove(tempList[0]);
            }
            else
            {
                List<LeaderboardData> tempList1 = SortLeaderBoardEnemiesDefeated(allLeaderboardList);
                if(tempList1.Count == 1)
                {
                    finalLeaderboardList.Add(tempList1[0]);
                    allLeaderboardList.Remove(tempList1[0]);
                }
                else
                {
                    List<LeaderboardData> tempList2 = SortLeaderBoardSurvivalTime(allLeaderboardList);
                    if(tempList2.Count == 1)
                    {
                        finalLeaderboardList.Add(tempList2[0]);
                        allLeaderboardList.Remove(tempList2[0]);
                    }
                    else
                    {
                        for (int i = 0; i < tempList2.Count; i++)
                        {
                            finalLeaderboardList.Add(tempList2[i]);
                            allLeaderboardList.Remove(tempList2[i]);
                            tempList2.RemoveAt(i);

                            i--;
                        }
                    }
                }
            }
        }

        int totalPlayer = finalLeaderboardList.Count;
        float playerRankPercentage = 0;
        for (int i = 0; i < finalLeaderboardList.Count; i++)
        {
            if (finalLeaderboardList[i] == playerLeaderboard)
            {
                playerRankPercentage = ((float)i + (float)1) / (float)totalPlayer;

                break;
            }
        }

        playerRankPercentage = playerRankPercentage * 100;
        playerRankPercentage = (float)Math.Round(playerRankPercentage, 2);

        playerRankInfo.text = $"You've made Top {playerRankPercentage}%";
    }
    
    public List<LeaderboardData> SortLeaderBoardSingleListCompletedRound(List<LeaderboardData> leaderboardDataList)
    {
        float highestValue = 0;
        List<LeaderboardData> tempLeaderboardDataList = new List<LeaderboardData>();
        foreach (LeaderboardData leaderboardData in leaderboardDataList)
        {
            if (leaderboardData.score > highestValue)
            {
                highestValue = leaderboardData.score;
                tempLeaderboardDataList.Clear();
                tempLeaderboardDataList.Add(leaderboardData);
            }
            else if (leaderboardData.score == highestValue)
                tempLeaderboardDataList.Add(leaderboardData);
        }

        return tempLeaderboardDataList;
    }

    public List<LeaderboardData> SortLeaderBoardEnemiesDefeated(List<LeaderboardData> leaderboardDataList)
    {
        float highestValue = 0;
        List<LeaderboardData> tempLeaderboardDataList = new List<LeaderboardData>();
        foreach (LeaderboardData leaderboardData in leaderboardDataList)
        {
            if (leaderboardData.defeatedEnemyNumberAndSurviveTimer[0] > highestValue)
            {
                highestValue = leaderboardData.defeatedEnemyNumberAndSurviveTimer[0];

                tempLeaderboardDataList.Clear();
                tempLeaderboardDataList.Add(leaderboardData);
            }
            else if (leaderboardData.defeatedEnemyNumberAndSurviveTimer[0] == highestValue)
                tempLeaderboardDataList.Add(leaderboardData);
        }

        return tempLeaderboardDataList;
    }
    public List<LeaderboardData> SortLeaderBoardSurvivalTime(List<LeaderboardData> leaderboardDataList)
    {
        float highestValue = 0;
        List<LeaderboardData> tempLeaderBoardSingleList = new List<LeaderboardData>();
        foreach (LeaderboardData leaderboardData in leaderboardDataList)
        {
            if (leaderboardData.defeatedEnemyNumberAndSurviveTimer[1] > highestValue)
            {
                highestValue = leaderboardData.defeatedEnemyNumberAndSurviveTimer[1];

                tempLeaderBoardSingleList.Clear();
                tempLeaderBoardSingleList.Add(leaderboardData);
            }
            else if (leaderboardData.defeatedEnemyNumberAndSurviveTimer[1] == highestValue)
                tempLeaderBoardSingleList.Add(leaderboardData);
        }

        return tempLeaderBoardSingleList;
    }
    
    public void SetLeaderboardUiActive(bool b)
    {
        if (b)
        {
            leaderboardCanvasGroup.alpha = 1f;
            leaderboardCanvasGroup.interactable = true;
            leaderboardCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            leaderboardCanvasGroup.alpha = 0f;
            leaderboardCanvasGroup.interactable = false;
            leaderboardCanvasGroup.blocksRaycasts = false;
        }
    }
}
