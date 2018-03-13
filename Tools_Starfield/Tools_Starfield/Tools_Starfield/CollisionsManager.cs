using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tools_Starfield
{
    class CollisionsManager
    {
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private ExplosionManager explosionManager;
        private Vector2 offScreen = new Vector2(-500, -500);

        public CollisionsManager(PlayerManager playerSprite, EnemyManager enemyManager/*, ExplosionManager explosionManager*/)
        {
            this.playerManager = playerSprite;
            //this.explosionManager = explosionManager;
            this.enemyManager = enemyManager;
        }

        private void checkShotToEnemyCollisions()
        {
            foreach ( Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                foreach ( Enemy enemy in enemyManager.Enemies)
                {
                    if(shot.IsCircleColliding(enemy.EnemySprite.Center,enemy.EnemySprite.collisionRadius))
                    {
                        shot.Position = offScreen;
                        enemy.Destroyed = true;

                        //explosionManager.AddExplosion(enemy.EnemySprite.Center, enemy.EnemySprite.Velocity / 10);
                    }
                }
            }
        }

        private void checkShotToPlayerCollisions()
        {
            foreach ( Sprite shot in enemyManager.EnemyShotManager.Shots)
            {
                if ( shot.IsCircleColliding(playerManager.Center, playerManager.CollisionRadius))
                {
                    shot.Position = offScreen;
                    playerManager.Destroyed = true;

                    //explosionManager.AddExplosion(playerManager.Center, Vector2.Zero);
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                if (enemy.EnemySprite.IsCircleColliding(playerManager.Position, playerManager.CollisionRadius))
                {
                    enemy.Destroyed = true;
                    //explosionManager.AddExplosion(enemy.EnemySprite.Center, enemy.EnemySprite.Velocity / 10);

                    playerManager.Destroyed = true;
                    //explosionManager.AddExplosion(playerManager.Center, Vector2.Zero);
                }
            }
        }

        public void CheckCollisions()
        {
            checkShotToEnemyCollisions();

            if (!playerManager.Destroyed)
            {
                checkShotToPlayerCollisions();
                checkEnemyToPlayerCollisions();
            }
        }
    }
}
