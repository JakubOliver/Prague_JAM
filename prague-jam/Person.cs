using Godot;

public enum Stages
{
	Idle, Run, Attack, Dead, Hit
}

public partial class Person : Area2D
{
	public Stages Stage = Stages.Idle;
	public  bool InCollision = false;
	public Person InCollisionWith = null;
	public Person player = null;
	public Person soldier = null;
	public Person wizard = null;
	public Vector2 ScreenSize;
	public AnimatedSprite2D AnimatedSprite2D;
	public int Health;
	public int Speed;
	public int Damage;

	public const double HIT_COOLDOWN_MAX = 0.5;
	public double HitCoolDown = HIT_COOLDOWN_MAX;

	public const double ATTACK_TIME_MAX = 0.6;
	public double TimeInAttack = 0.0;

	public const double ATTACK_COOLDOWN_MAX = 0.4;
	public double AttackCoolDown = 0.0;

	public bool AlreadyInAttack = false;

	protected AnimationPlayer AnimationPlayer;
	protected AudioStreamPlayer2D sfx_death;
	protected AudioStreamPlayer2D sfx_hit;

	protected void GetTransition()
	{
		AnimationPlayer = GetParent().GetNode<AnimationPlayer>("Transition/AnimationPlayer");
	}
	protected void GetDeathSound()
	{
		sfx_death = GetNode<AudioStreamPlayer2D>("sfx_death");
	}
	protected void GetHitSound()
	{
		sfx_hit = GetNode<AudioStreamPlayer2D>("sfx_hit");
	}

	public void DoHit(Person target, Person target2)
	{
		ChangeAnimation(Stages.Attack);

		if (target != null)
		{
			target.GetHit(Damage);
		}

		if (target2 != null)
		{
			target2.GetHit(Damage);
		}
	}

	public void DoHit(Person target)
	{
		DoHit(target, null);
	}

	virtual protected async void Dead()
	{
		
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		AnimationPlayer.Play("fade_in");
		//await ToSignal(AnimationPlayer, "animation_finished");

		GetTree().ReloadCurrentScene();
	}

	public void GetHit(int damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			//sfx_death.Play();
			ChangeAnimation(Stages.Dead);
			if (player != null)
			{
				player.ChangeAnimation(Stages.Idle);
				Dead();
				return;
			}

			if (this is Wizard || this is Soldier)
			{
				//sfx_death.Play();
				Dead();
			} else if (this is Player)
			{
				if (soldier != null)
				{
					soldier.ChangeAnimation(Stages.Idle);
				}
				Dead();
			}
		}
		else
		{
			ChangeAnimation(Stages.Hit);
			HitCoolDown = HIT_COOLDOWN_MAX;
		}
	}

	protected void ProcessHitCoolDown(double delta)
	{
		HitCoolDown -= delta;
		if (HitCoolDown <= 0)
		{
			HitCoolDown = 0;
			ChangeAnimation(Stages.Idle);
		}
	}

	protected void ChangeAnimation(Stages newStage)
	{
		if (Stage == newStage) return;

		Stage = newStage;
		switch (Stage)
		{
			case Stages.Idle:
				AnimatedSprite2D.Play("idle");
				break;
			case Stages.Run:
				AnimatedSprite2D.Play("run");
				break;
			case Stages.Attack:
				AnimatedSprite2D.Play("attack");
				if (!AlreadyInAttack)
				{
					AlreadyInAttack = true;
					AnimatedSprite2D.AnimationFinished += () =>
					{
						if (Stage == Stages.Attack)
						{
							ChangeAnimation(Stages.Idle);
							AlreadyInAttack = false;
						}
					};
				}
				break;
			case Stages.Hit:
				AnimatedSprite2D.Play("hit");
				sfx_hit.Play();
				break;
			case Stages.Dead:
				AnimatedSprite2D.Play("death");
				sfx_death.Play();
				break;
		}
	}
}
