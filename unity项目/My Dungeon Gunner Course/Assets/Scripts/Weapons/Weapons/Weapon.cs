using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon 和 WeaponDetailsSO 的区别?
/// WeaponDetailsSO存放武器的各个数据 在编辑器内可以让策划自行配置（数据驱动）
/// Weapon有着对WeaponDetailsSO的引用
/// 在代码中其他类要引用武器时引用的是Weapon类而不是WeaponDetailsSO 相当于间接引用
///
/// 按照这种设计思路 可以实现其他的一些构思
/// eg. 武器有着不同的弹药类型（普通子弹 火箭弹 霰弹枪等等）
/// 不同弹药有着不同的属性（材质 伤害 飞行速度 轨迹等等） 这些属性在AmmoDetailsSO中配置
/// Ammo类则存放对AmmoDetailsSO的引用
/// 其他类要获取弹药信息时 得先获取Ammo类的引用 再间接引用AmmoDetailsSO的数据
/// </summary>
public class Weapon
{
    public WeaponDetailsSO weaponDetails;       // 武器信息SO
    public int weaponListPosition;              // 武器池Position
    public float weaponReloadTimer;             // 武器清空弹夹后的加载时间
    public int weaponClipRemainingAmmo;         // 武器弹夹剩余弹药数量
    public int weaponRemainingAmmo;             // 武器剩余弹药总数量
    public bool isWeaponReloading;              // 武器是否在加载
    
}
