using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Reflection;

public class Enemy: EnemyBase{
	public int CurrentHItPoint = 0;

	public Enemy(EnemyBase ebase){
		List<PropertyInfo> props = ebase
			.GetType()
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)?
			.ToList();

		props.ForEach(prop =>	{
			var propValue = prop.GetValue(ebase);
			typeof(Enemy).GetProperty(prop.Name).SetValue(this, propValue);
		});
	}
}
