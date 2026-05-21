

//인터페이스
using System;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

interface IUsable
{
    void Use(Player player);
}

// 아이템 클래스

class Item
{
    // 아이템 이름
    public string Name;
}

// 장비 클래스

// 무기 클래스
class Weapon : Item
{
    public int AttackBonus;  // 추가 공격력

    public Weapon(string name, int attack)
    {
        Name = name;
        AttackBonus = attack;
    }
}

// 방어구 클래스
class Armor : Item
{
    public int HpBonus; // 추가 체력

    public Armor(string name, int hp)
    {
        Name = name;
        HpBonus = hp;
    }
}

// 포션

class Potion : Item, IUsable
{
    public int HealAmount;  // 회복량

    public Potion(string name, int heal)
    {
        Name = name;
        HealAmount = heal;
    }

    // 포션 사용
    public void Use(Player player)
    {
        player.Hp += HealAmount;

        Console.WriteLine($"{Name} 사용!");
        Console.WriteLine($"{HealAmount} 회복!");
    }
}

// 캐릭터 추상 클래스

abstract class Character
{
    public string Name;
    public int Hp;
    public int Attack;
    public bool IsPoison;

    public Character(string name, int hp, int attack)
    {
        Name = name;
        Hp = hp;
        Attack = attack;
    }

    // 공격

    public virtual void AttackTarget(Character target)
    {
        int damage = Attack;

        Random rand = new Random();

        // 치명타
        if (rand.Next(100) < 20)
        {
            damage *= 2;

            Console.WriteLine("치명타 발생!");
        }

        target.Hp -= damage;

        Console.WriteLine($"{Name}이(가) {target.Name}에게 {damage} 데미지!");
    }

    // 상태 이상 처리

    public void StatusEffect()
    {
        if (IsPoison)
        {
            Hp -= 5;

            Console.WriteLine($"{Name} 독 데미지 5!");
        }
    }
    // 사망 확인
    public bool IsDead()
    {
        return Hp <= 0;
    }
    public virtual void ShowStatus()
    {
        Console.WriteLine($"{Name} HP : {Hp}");
    }
}

// 플레이어 클래스

class Player : Character
{
    public int Level = 1;
    public int Exp = 0;
    public int Gold = 100;

    public List<Item> Inventory = new List<Item>();

    //장착 아이템
    public Weapon EquipWeapon;

    public Armor EquipArmor;

    public Player(string name): base(name, 100, 15)
    {

    }

    // 경험치 획득

    public void GainExp(int exp)
    {
        Exp += exp;

        Console.WriteLine($"{exp} 경험치 획득!");

        // 레벨업 조건
        if (Exp >= Level * 20)
        {
            LevelUp();
        }
    }

    // 레벨업

    public void LevelUp()
    {
        Level++;

        Hp += 20;
        Attack += 5;

        Console.WriteLine("레벨 업!");
    }
    // 장비 장착

    public void EquipItem(Item item)
    {
        // 무기 장착
        if (item is Weapon)
        {
            Weapon weapon = item as Weapon;

            EquipWeapon = weapon;

            Attack += weapon.AttackBonus;

            Console.WriteLine($"{weapon.Name} 장착!");
        }

        // 방어구 장착
        else if (item is Armor)
        {
            Armor armor = item as Armor;

            EquipArmor = armor;

            Hp += armor.HpBonus;

            Console.WriteLine($"{armor.Name} 장착!");
        }
    }

    // 상태 출력

    public override void ShowStatus()
    {
        Console.WriteLine("\n===== 플레이어 상태 =====");

        Console.WriteLine($"이름 : {Name}");
        Console.WriteLine($"레벨 : {Level}");
        Console.WriteLine($"HP : {Hp}");
        Console.WriteLine($"공격력 : {Attack}");
        Console.WriteLine($"EXP : {Exp}");
        Console.WriteLine($"Gold : {Gold}");

        // 장착 장비 출력
        if (EquipWeapon != null)
        {
            
            Console.WriteLine($"무기 : {EquipWeapon.Name}");
        }

        if (EquipArmor != null)
        {
            
            Console.WriteLine($"방어구 : {EquipArmor.Name}");
        }
    }
}

// 몬스터 클래스
class Monster : Character
{
    // 경험치 보상
    public int RewardExp;

    // 골드 보상
    public int RewardGold;

    // 보스 여부
    public bool IsBoss;

    public Monster(string name, int hp, int attack, int exp, int gold, bool boss) : base(name, hp, attack)
    {
        RewardExp = exp;
        RewardGold = gold;
        IsBoss = boss;
    }
}

// 전투 관리자
class BattleManager
{
    // 전투 시작
    public void Battle(Player player, Monster monster)
    {
        Console.Clear();

        Console.WriteLine($"{monster.Name} 등장!");

        while (!player.IsDead() && !monster.IsDead())
        {
            Console.WriteLine("\n1. 공격");
            Console.WriteLine("2. 포션 사용");

            int input =int.Parse(Console.ReadLine());

            Console.Clear();

            // 상태 이상 처리
            player.StatusEffect();
            monster.StatusEffect();

            // 공격
            if (input == 1)
            {
                player.AttackTarget(monster);

                Random rand = new Random();

                if (rand.Next(100) < 20)
                {
                    monster.IsPoison = true;

                    Console.WriteLine("몬스터 중독!");
                }
            }

            // 포션 사용
            else if (input == 2)
            {
                UsePotion(player);
            }

            // 몬스터 생존 시 반격
            if (!monster.IsDead())
            {
                monster.AttackTarget(player);
            }

            Console.WriteLine();

            player.ShowStatus();
            monster.ShowStatus();
        }

        // 승리
        if (monster.IsDead())
        {
            Console.WriteLine($"\n{monster.Name} 처치!");

            player.GainExp(monster.RewardExp);

            player.Gold += monster.RewardGold;

            Console.WriteLine($"{monster.RewardGold} Gold 획득!");

            // 아이템 드랍
            DropItem(player);
        }
        // 패배
        if (player.IsDead())
        {
            Console.WriteLine("게임 오버");
        }

        Console.ReadLine();
    }

    // 포션 사용
    void UsePotion(Player player)
    {
        Console.WriteLine("\n===== 인벤토리 =====");

        for (int i = 0;i < player.Inventory.Count;i++)
        {
            Console.WriteLine($"{i}. {player.Inventory[i].Name}");
        }

        int select = int.Parse(Console.ReadLine());

        IUsable item = player.Inventory[select] as IUsable;

        if (item != null)
        {
            item.Use(player);
            player.Inventory.RemoveAt(select);
        }
    }

    // 아이템 드랍
    void DropItem(Player player)
    {
        Random rand = new Random();

        int chance = rand.Next(100);

        // 포션 드랍
        if (chance < 40)
        {
            Potion potion = new Potion("회복 포션", 30);

            player.Inventory.Add(potion);

            Console.WriteLine("회복 포션 획득!");
        }

        // 무기 드랍
        else if (chance < 70)
        {
            Weapon weapon = new Weapon("철 검", 5);

            player.Inventory.Add(weapon);

            Console.WriteLine("철 검 획득!");
        }

        // 방어구 드랍
        else
        {
            Armor armor = new Armor("철 갑옷", 20);
            player.Inventory.Add(armor);
            Console.WriteLine("철 갑옷 획득!");
        }
    }
}

// 게임 관리자
class GameManager
{
    Player player;

    // 몬스터 리스트
    List<Monster> monsters = new List<Monster>();

    BattleManager battleManager = new BattleManager();

    // 게임 시작
    public void Start()
    {
        Console.Write("이름 입력 : ");

        string name =
            Console.ReadLine();

        player = new Player(name);

        // 몬스터 추가
        monsters.Add(
            new Monster("슬라임", 30, 5, 10, 20, false));

        monsters.Add(
            new Monster("고블린", 50, 10, 20, 30, false));

        monsters.Add(
            new Monster("오크", 80, 15, 30, 50, false));

        monsters.Add(
            new Monster("드래곤", 200, 30, 100, 200, true));

        // 게임 반복
        while (!player.IsDead())
        {
            Console.Clear();

            Console.WriteLine("===== 마을 =====");

            Console.WriteLine("1. 숲");
            Console.WriteLine("2. 던전");
            Console.WriteLine("3. 보스방");
            Console.WriteLine("4. 상태 보기");
            Console.WriteLine("5. 인벤토리");
            Console.WriteLine("6. 상점");
            Console.WriteLine("7. 종료");

            int input = int.Parse(Console.ReadLine());

            Console.Clear();

            // 숲
            if (input == 1)
            {
                EnterForest();
            }

            // 던전
            else if (input == 2)
            {
                EnterDungeon();
            }

            // 보스방
            else if (input == 3)
            {
                EnterBossRoom();
            }

            // 상태 보기
            else if (input == 4)
            {
                player.ShowStatus();

                Console.ReadLine();
            }

            // 인벤토리
            else if (input == 5)
            {
                ShowInventory();
            }

            // 상점
            else if (input == 6)
            {
                Shop();
            }

            // 종료
            else if (input == 7)
            {
                break;
            }
        }
    }

    // 숲 이동

    void EnterForest()
    {
        Console.WriteLine("숲으로 이동!");

        Monster data = monsters[0];
        
        Monster monster = new Monster(data.Name, data.Hp, data.Attack, data.RewardExp, data.RewardGold, data.IsBoss);
        battleManager.Battle(player, monster);
    }

    // 던전 이동

    void EnterDungeon()
    {
        Console.WriteLine("던전 이동!");

        Random rand = new Random();

        int index = rand.Next(3);

        Monster data = monsters[index];
        Monster monster = new Monster(data.Name,data.Hp,data.Attack,data.RewardExp,data.RewardGold,data.IsBoss);

        battleManager.Battle(player, monster);
    }

    // 보스방 이동
    void EnterBossRoom()
    {
        Console.WriteLine("보스방 이동!");

        Monster data = monsters[3];
       
        Monster boss = new Monster(data.Name, data.Hp, data.Attack, data.RewardExp, data.RewardGold, data.IsBoss);
        battleManager.Battle(player, boss);
    }

    // 인벤토리 출력

    void ShowInventory()
    {
        Console.WriteLine("===== 인벤토리 =====");

        for (int i = 0;i < player.Inventory.Count;i++)
        {
            Console.WriteLine($"{i}. {player.Inventory[i].Name}");
        }

        Console.WriteLine("\n장착할 번호 입력");
        Console.WriteLine("-1 입력 시 종료");

        int input = int.Parse(Console.ReadLine());

        if (input >= 0)
        {
            player.EquipItem(player.Inventory[input]);
        }
    }
    // 상점

    void Shop()
    {
        Console.WriteLine("===== 상점 =====");

        Console.WriteLine("1. 회복 포션 구매 (30G)");
        Console.WriteLine("2. 철 검 구매 (150G)");
        Console.WriteLine("3. 철 갑옷 구매 (200G)");

        int input = int.Parse(Console.ReadLine());

        // 포션 구매
        if (input == 1 && player.Gold >= 30)
        {
            player.Gold -= 30;
            player.Inventory.Add(new Potion("회복 포션", 30));
            Console.WriteLine("구매 완료!");
        }

        // 무기 구매
        else if (input == 2 && player.Gold >= 150)
        {
            player.Gold -= 150;
            player.Inventory.Add(new Weapon("철 검", 5));
            Console.WriteLine("구매 완료!");
        }

        // 방어구 구매
        else if (input == 3 && player.Gold >= 200)
        {
            player.Gold -= 200;
            player.Inventory.Add(new Armor("철 갑옷", 20));
            Console.WriteLine("구매 완료!");
        }

        else
        {
            Console.WriteLine("Gold 부족!");
        }

        Console.ReadLine();
    }
}

// 메인

class Program
{
    static void Main(string[] args)
    {
        GameManager game =
            new GameManager();

        game.Start();
    }
}