/// <summary>
/// マージソートクラス
/// 
/// 2014/07/11
/// </summary>
using System;
using System.Collections.Generic;

/// <summary>
/// マージソートの処理を行うクラス
/// 主に安定性が必要なソートを行う場合に使用する
/// (不安定性のソートが必要な場合はコレクション内にあるソートの方がマージソートを使用するより速度は速い)
/// </summary>
static public class MargeSortCollection
{
    #region 配列版

    /// <summary>
    /// 配列全体内の要素を並び替える
    /// </summary>
    /// <param name="target">
    /// 並び替える対象の配列
    /// </param>
    static public void ArraySort<T>(T[] target) where T : IComparable<T>
    {
        T[] work = new T[target.Length / 2];
        Sort(target, work, 0, target.Length);
    }

    /// <summary>
    /// 指定したSystem.Comparison<T>を使用して配列全体内の要素を並び替える
    /// </summary>
    /// <param name="target">
    /// 並び替える対象の配列
    /// </param>
    /// <param name="comparer">
    /// 比較するメソッド
    /// </param>
    static public void ArraySort<T>(T[] target, Comparison<T> comparer)
    {
        T[] work = new T[target.Length / 2];
        Sort(target, work, 0, target.Length, comparer);
    }

    /// <summary>
    /// 各データ列がデータ数2以下になるまで分割しマージする処理
    /// IComparable版
    /// </summary>
    static private void Sort<T>(T[] target, T[] work, int begin, int end) where T : IComparable<T>
    {
        if (end - begin < 2)
            return;

        // 半分値を求める
        int mid = (begin + end) / 2;

        // 2つになるまで分解していく
        Sort(target, work, begin, mid);
        Sort(target, work, mid, end);

        // 2つのデータ列をマージ
        Marge(target, work, begin, mid, end);

        return;
    }

    /// <summary>
    /// 各データ列がデータ数2以下になるまで分割しマージする処理
    /// Comparison版
    /// </summary>
    static private void Sort<T>(T[] target, T[] work, int begin, int end, Comparison<T> comparer)
    {
        if (end - begin < 2)
            return;

        // 半分値を求める
        int mid = (begin + end) / 2;

        // 2つになるまで分解していく
        Sort(target, work, begin, mid, comparer);
        Sort(target, work, mid, end, comparer);

        // 2つの配列をマージ
        Marge(target, work, begin, mid, end, comparer);

        return;
    }

    /// <summary>
    /// 2つのデータ列をマージする処理
    /// IComparable版
    /// </summary>
    static private void Marge<T>(T[] target, T[] work, int begin, int mid, int end) where T : IComparable<T>
    {
        int i, j, k;

        for (i = begin, j = 0; i < mid; ++i, ++j)
        {
            work[j] = target[i];
        }

        mid -= begin;
        for (j = 0, k = begin; i < end && j < mid; ++k)
        {
            if (target[i].CompareTo(work[j]) < 0)
            {
                target[k] = target[i];
                i++;
            }
            else
            {
                target[k] = work[j];
                j++;
            }
        }

        for (; i < mid; ++i, ++k)
        {
            target[k] = target[i];
        }
        for (; j < mid; ++j, ++k)
        {
            target[k] = work[j];
        }
    }

    /// <summary>
    /// 2つのデータ列をマージする処理
    /// Comparison版
    /// </summary>
    static private void Marge<T>(T[] target, T[] work, int begin, int mid, int end, Comparison<T> iComparer)
    {
        int i, j, k;

        for (i = begin, j = 0; i < mid; ++i, ++j)
        {
            work[j] = target[i];
        }

        mid -= begin;
        for (j = 0, k = begin; i < end && j < mid; ++k)
        {
            if (iComparer(target[i], work[j]) < 0)
            {
                target[k] = target[i];
                i++;
            }
            else
            {
                target[k] = work[j];
                j++;
            }
        }

        for (; i < mid; ++i, ++k)
        {
            target[k] = target[i];
        }
        for (; j < mid; ++j, ++k)
        {
            target[k] = work[j];
        }
    }

    #endregion

    #region List版

    /// <summary>
    /// List全体内の要素を並び替える
    /// </summary>
    /// <param name="target">
    /// 並び替える対象のList
    /// </param>
    static public void MargeSort<T>(this List<T> target) where T : IComparable<T>
    {
        T[] work = new T[target.Count / 2];
        Sort(target, work, 0, target.Count);
    }

    /// <summary>
    /// 指定したSystem.Comparison<T>を使用してList全体内の要素を並び替える
    /// </summary>
    /// <param name="target">
    /// 並び替える対象のList
    /// </param>
    /// <param name="comparer">
    /// 比較するメソッド
    /// </param>
    static public void MargeSort<T>(this List<T> target, Comparison<T> comparer)
    {
        T[] work = new T[target.Count / 2];
        Sort(target, work, 0, target.Count, comparer);
    }

    /// <summary>
    /// 各データ列がデータ数2以下になるまで分割しマージする処理
    /// IComparable版
    /// </summary>
    static private void Sort<T>(this List<T> target, T[] work, int begin, int end) where T : IComparable<T>
    {
        if (end - begin < 2)
            return;

        // 半分値を求める
        int mid = (begin + end) / 2;

        // 2つになるまで分解していく
        Sort(target, work, begin, mid);
        Sort(target, work, mid, end);

        // 2つのデータ列をマージ
        Marge(target, work, begin, mid, end);

        return;
    }

    /// <summary>
    /// 各データ列がデータ数2以下になるまで分割しマージする処理
    /// Comparison版
    /// </summary>
    static private void Sort<T>(List<T> target, T[] work, int begin, int end, Comparison<T> comparer)
    {
        if (end - begin < 2)
            return;

        // 半分値を求める
        int mid = (begin + end) / 2;

        // 2つになるまで分解していく
        Sort(target, work, begin, mid, comparer);
        Sort(target, work, mid, end, comparer);

        // 2つの配列をマージ
        Marge(target, work, begin, mid, end, comparer);

        return;
    }

    /// <summary>
    /// 2つのデータ列をマージする処理
    /// IComparable版
    /// </summary>
    static private void Marge<T>(List<T> target, T[] work, int begin, int mid, int end) where T : IComparable<T>
    {
        int i, j, k;

        for (i = begin, j = 0; i < mid; ++i, ++j)
        {
            work[j] = target[i];
        }

        mid -= begin;
        for (j = 0, k = begin; i < end && j < mid; ++k)
        {
            if (target[i].CompareTo(work[j]) < 0)
            {
                target[k] = target[i];
                i++;
            }
            else
            {
                target[k] = work[j];
                j++;
            }
        }

        for (; i < mid; ++i, ++k)
        {
            target[k] = target[i];
        }
        for (; j < mid; ++j, ++k)
        {
            target[k] = work[j];
        }
    }

    /// <summary>
    /// 2つのデータ列をマージする処理
    /// Comparison版
    /// </summary>
    static private void Marge<T>(List<T> target, T[] work, int begin, int mid, int end, Comparison<T> iComparer)
    {
        int i, j, k;

        for (i = begin, j = 0; i < mid; ++i, ++j)
        {
            work[j] = target[i];
        }

        mid -= begin;
        for (j = 0, k = begin; i < end && j < mid; ++k)
        {
            if (iComparer(target[i], work[j]) < 0)
            {
                target[k] = target[i];
                i++;
            }
            else
            {
                target[k] = work[j];
                j++;
            }
        }

        for (; i < mid; ++i, ++k)
        {
            target[k] = target[i];
        }
        for (; j < mid; ++j, ++k)
        {
            target[k] = work[j];
        }
    }

    #endregion
}
