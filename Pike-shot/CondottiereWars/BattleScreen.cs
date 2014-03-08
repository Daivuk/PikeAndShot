using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

namespace PikeAndShot
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BattleScreen
    {
        public const int SIDE_PLAYER = 1;
        public const int SIDE_NEUTRAL = 0;
        public const int SIDE_ENEMY = -1;

        public const float SCROLLPOINT = 0.33f;
        public const float BATTLEHEIGHTEXTEND = 384f;

        private bool _drawDots;

        protected ArrayList _shots;
        protected ArrayList _deadThings;
        protected ArrayList _newThings;
        protected ArrayList _deadFormations;
        protected ArrayList _looseSoldiers;
        protected ArrayList _screenObjects;
        protected ArrayList _screenObjectsToAdd;
        protected ArrayList _screenColliders;
        protected ArrayList _screenAnimations;
        protected ArrayList _enemyFormations;
        protected ArrayList _terrain;
        protected ArrayList _drawJobs;

        public PikeAndShotGame _game;

        protected Formation _formation;
        protected KeyboardState keyboardState;
        protected KeyboardState previousKeyboardState;
        protected GamePadState gamePadState;
        protected GamePadState previousGamePadState;

        protected double _elapsedTime;
        protected Vector2 _mapOffset;
        protected bool _active;

        public BattleScreen(PikeAndShotGame game)
        {
            _game = game;
            _active = false;

            //shot and clean up arrays
            _shots = new ArrayList(60);
            _deadThings = new ArrayList(60);
            _newThings = new ArrayList(60);
            _deadFormations = new ArrayList(60);

            //arrays for collisions
            _screenObjects = new ArrayList(60);
            _screenObjectsToAdd = new ArrayList(60);
            _screenColliders = new ArrayList(60);
            _screenAnimations = new ArrayList(60);

            _enemyFormations = new ArrayList(25);
            _looseSoldiers = new ArrayList(60);
            
            previousKeyboardState = Keyboard.GetState();
            _elapsedTime = 0.0;

            _mapOffset = new Vector2(0f, 0f);
            _drawDots = false;
            _terrain = new ArrayList(20);

            for (int i = 0; i < 100; i++)
            {
                _terrain.Add(new Terrain(this, PikeAndShotGame.ROAD_TERRAIN[PikeAndShotGame.random.Next(7)], SIDE_PLAYER, PikeAndShotGame.random.Next(PikeAndShotGame.SCREENWIDTH), PikeAndShotGame.random.Next(PikeAndShotGame.SCREENHEIGHT)));
            }

            _drawJobs = new ArrayList(255);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void update(GameTime gameTime)
        {
            getInput(gameTime.ElapsedGameTime);

            // check for screen change
            if (keyboardState.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _game.setScreen(PikeAndShotGame.SCREEN_LEVELPLAY);
            }
            else if (keyboardState.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _game.setScreen(PikeAndShotGame.SCREEN_FORMATIONMAKER);
            }
            else if (keyboardState.IsKeyDown(Keys.D3) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _game.setScreen(PikeAndShotGame.SCREEN_LEVELEDITOR);
            }

            _elapsedTime = gameTime.TotalGameTime.TotalMilliseconds;

            foreach (Shot shot in _shots)
            {
                shot.update(gameTime.ElapsedGameTime);
                if (shot.isDead())
                    _deadThings.Add(shot);
            }

            if(_formation != null)
                _formation.update(gameTime.ElapsedGameTime);

            foreach (ScreenObject screeny in _screenObjectsToAdd)
            {
                _screenObjects.Add(screeny);
            }
            _screenObjectsToAdd.Clear();

            foreach (Formation f in _enemyFormations)
            {
                f.update(gameTime.ElapsedGameTime);
                if (f.getPosition().X < (-1 * f.getTotalRows() * Soldier.WIDTH) + _mapOffset.X || f.getTotalRows() == 0)
                {
                    if(!f.hasSoldierOnScreen() && this is LevelScreen)
                        _deadFormations.Add(f);
                }
            }
            
            //checking for empty or off screen formations
            foreach (Formation f in _deadFormations)
            {
                _enemyFormations.Remove(f);
                foreach (Soldier sold in f.getSoldiers())
                    _deadThings.Add(sold);
            }
            _deadFormations.Clear();
            
            checkCollisions();

            foreach (Soldier sold in _looseSoldiers)
            {
                sold.update(gameTime.ElapsedGameTime);

                if (sold.getSide() == BattleScreen.SIDE_PLAYER && sold.getPosition().X > PikeAndShotGame.SCREENWIDTH + getMapOffset().X + 2f * Soldier.WIDTH)
                    sold.setState(Soldier.STATE_DEAD);
                else if (sold.getSide() == BattleScreen.SIDE_ENEMY && sold.getPosition().X < -2f * Soldier.WIDTH + getMapOffset().X)
                    sold.setState(Soldier.STATE_DEAD);

                if (sold.isDead())
                {
                    _deadThings.Add(sold);
                }
            }

            foreach (Terrain t in _terrain)
            {
                if (t.isAnimated())
                {
                    t.update(gameTime.ElapsedGameTime);
                }

                if (t.getPosition().X < (-2f * Soldier.WIDTH) + getMapOffset().X)
                    t.setState(Soldier.STATE_DEAD);

                if (t.isDead())
                {
                    _deadThings.Add(t);
                }
            }

            foreach (Soldier newThing in _newThings)
            {
                _looseSoldiers.Add(newThing);
            }
            _newThings.Clear();

            // clean up of dead objects
            foreach (ScreenObject obj in _deadThings)
            {
                if (obj is Shot)
                    _shots.Remove(obj);
                else if (obj is Soldier)
                    _looseSoldiers.Remove(obj);
                else if (obj is Terrain)
                {
                    _terrain.Remove(obj);
                    _terrain.Add(new Terrain(this, PikeAndShotGame.ROAD_TERRAIN[PikeAndShotGame.random.Next(7)], SIDE_PLAYER, PikeAndShotGame.SCREENWIDTH + getMapOffset().X, PikeAndShotGame.random.Next(PikeAndShotGame.SCREENHEIGHT)));
                }

                _screenObjects.Remove(obj);
            }
            _deadThings.Clear();

            // screen animations

            foreach (ScreenAnimation ani in _screenAnimations)
            {
                ani.update(gameTime.ElapsedGameTime);
                if (ani.isDone())
                    _deadThings.Add(ani);
            }

            foreach (ScreenAnimation ani in _deadThings)
            {
                _screenAnimations.Remove(ani);
            }

            _deadThings.Clear();
        }

        public void addScreenObject(ScreenObject so)
        {
            _screenObjectsToAdd.Add(so);
            //_screenObjects.Add(so);
        }

        public Formation getPlayerFormation()
        {
            return _formation;
        }

        public Vector2 getMapOffset()
        {
            return _mapOffset;
        }

        protected void checkCollisions()
        {
            int x = 0;

            float soX, soY, soWidth, soHeight;
            float coX, coY, coWidth, coHeight;
            
            bool collision = true;
            bool oneCollision = false;

            _screenColliders.Clear();

            // pour all of the screen objects into the list of objects to check for collisions against to start with
            foreach (ScreenObject so in _screenObjects)
            {
                if (so.getState() != ScreenObject.STATE_DEAD && so.getState() != ScreenObject.STATE_DYING)
                    _screenColliders.Add(so);
            }
            
            // Now for every object see if it hit any of the colliders
            // screenobjects that didn't hit anything can be removed from the list of coliders so they aren't checked repeatedly for no reason
            foreach (ScreenObject so in _screenObjects)
            {
                if (so is WeaponSwing)
                    x++;
                if (so.getState() != ScreenObject.STATE_DEAD || so.getState() != ScreenObject.STATE_DYING)
                {
                    // get the values here so we aren't calling functions like crazy
                    // pavise HACK
                    if (so is Pavise)
                    {
                        soX = so.getPosition().X;
                        soY = so.getPosition().Y - 10;
                        soWidth = so.getWidth();
                        soHeight = so.getHeight() + 10;
                    }
                    else
                    {
                        soX = so.getPosition().X;
                        soY = so.getPosition().Y;
                        soWidth = so.getWidth();
                        soHeight = so.getHeight();
                    }
                    foreach (ScreenObject co in _screenColliders)
                    {
                        if (so != co)
                        {
                            // pavise HACK
                            if (so is Pavise)
                            {
                                coX = co.getPosition().X;
                                coY = co.getPosition().Y - 10;
                                coWidth = co.getWidth();
                                coHeight = co.getHeight() + 10;
                            }
                            else
                            {
                                coX = co.getPosition().X;
                                coY = co.getPosition().Y;
                                coWidth = co.getWidth();
                                coHeight = co.getHeight();
                            }

                            collision = true;

                            // see if we didn't collide
                            if (soX > coX + coWidth)
                                collision = false;
                            else if (soX + soWidth < coX)
                                collision = false;
                            else if (soY > coY + coHeight)
                                collision = false;
                            else if (soY + soHeight < coY)
                                collision = false;

                            if (collision)
                            {
                                so.collide(co);
                                oneCollision = true;
                                co.collide(so);
                            }
                        }
                    }
                    if (!oneCollision)
                        _screenColliders.Remove(so);
                }
            }
        }

        public void checkNonFatalCollision(ScreenObject so)
        {
            float soX, soY, soWidth, soHeight;
            float coX, coY, coWidth, coHeight;

            bool collision = true;

            _screenColliders.Clear();

            // pour all of the screen objects into the list of objects to check for collisions against to start with
            foreach (ScreenObject sObj in _screenObjects)
            {
                if (!(sObj is Soldier) && so.getState() != ScreenObject.STATE_DEAD && so.getState() != ScreenObject.STATE_DYING)
                    _screenColliders.Add(sObj);
            }
            
            if (so.getState() != ScreenObject.STATE_DEAD || so.getState() != ScreenObject.STATE_DYING)
            {
                // get the values here so we aren't calling functions like crazy
                // pavise HACK
                if (so is Pavise)
                {
                    soX = so.getPosition().X;
                    soY = so.getPosition().Y - 10;
                    soWidth = so.getWidth();
                    soHeight = so.getHeight() + 10;
                }
                else
                {
                    soX = so.getPosition().X;
                    soY = so.getPosition().Y;
                    soWidth = so.getWidth();
                    soHeight = so.getHeight();
                }
                foreach (ScreenObject co in _screenColliders)
                {
                    if (so != co)
                    {
                        // pavise HACK
                        if (so is Pavise)
                        {
                            coX = co.getPosition().X;
                            coY = co.getPosition().Y - 10;
                            coWidth = co.getWidth();
                            coHeight = co.getHeight() + 10;
                        }
                        else
                        {
                            coX = co.getPosition().X;
                            coY = co.getPosition().Y;
                            coWidth = co.getWidth();
                            coHeight = co.getHeight();
                        }

                        collision = true;

                        // see if we didn't collide
                        if (soX > coX + coWidth)
                            collision = false;
                        else if (soX + soWidth < coX)
                            collision = false;
                        else if (soY > coY + coHeight)
                            collision = false;
                        else if (soY + soHeight < coY)
                            collision = false;

                        if (collision)
                        {
                            so.collide(co);
                            co.collide(so);
                        }
                    }
                }
            }
            
        }

        public void removeScreenObject(ScreenObject so)
        {
            _deadThings.Add(so);
            //_screenObjects.Remove(so);
        }

        public void removeEnemyFormation(Formation enemyFormation)
        {
            _enemyFormations.Remove(enemyFormation);
        }

        protected virtual void getInput(TimeSpan timeSpan)
        {
            
        }

        public KeyboardState getPreviousKeyboardState()
        {
            return previousKeyboardState;
        }

        public GamePadState getPreviousGamePadState()
        {
            return previousGamePadState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(_formation != null)
                _formation.draw(spriteBatch);

            foreach (Formation f in _enemyFormations)
            {
                f.draw(spriteBatch);
            }

            foreach (Soldier sold in _looseSoldiers)
            {
                sold.draw(spriteBatch);
            }

            foreach (Shot shot in _shots)
            {
                shot.draw(spriteBatch);
            }

            foreach (ScreenAnimation ani in _screenAnimations)
            {
                ani.draw(spriteBatch);
            }

            foreach (Terrain t in _terrain)
            {
                t.draw(spriteBatch);
            }

            foreach (DrawJob dj in _drawJobs)
            {
                dj.sprite.draw(spriteBatch, dj.position, dj.side);
            }
            _drawJobs.Clear();
        }

        public void addDrawjob(DrawJob dj)
        {
            for (int i = 0; i < _drawJobs.Count; i++)
            {
                if (dj.drawingY < ((DrawJob)_drawJobs[i]).drawingY)
                {
                    _drawJobs.Insert(i, dj);
                    return;
                }
            }
            _drawJobs.Add(dj);
        }

        public void addShot(Shot shot)
        {
            _shots.Add(shot);
        }

        internal void addLooseSoldier(Soldier sold)
        {
            if(!_looseSoldiers.Contains(sold))
                _looseSoldiers.Add(sold);
        }

        internal void addLooseSoldierNext(Soldier sold)
        {
            if (!_newThings.Contains(sold))
                _newThings.Add(sold);
        }

        internal void removeLooseSoldier(Soldier sold)
        {
            if (_looseSoldiers.Contains(sold))
                _looseSoldiers.Remove(sold);
        }

        internal void addAnimation(ScreenAnimation screenAnimation)
        {
            _screenAnimations.Add(screenAnimation);
        }

        internal bool getDrawDots()
        {
            return _drawDots;
        }

        internal void toggleDrawDots()
        {
            _drawDots = !_drawDots;
        }


        internal bool findPikeTip(Soldier soldier, float range)
        {
            if(soldier.DEBUGFOUNDPIKE)
                soldier.DEBUGFOUNDPIKE = false;

            foreach (ScreenObject pt in _screenObjects)
            {
                if(pt is PikeTip)
                {
                    if(((PikeTip)pt).getSoldierState() != Pikeman.STATE_RECOILING || soldier.getState() == Targeteer.STATE_DEFEND)
                    {
                        // figure out the center of the pike tip and the center of the man
                        float ptX = pt.getPosition().X + pt.getWidth() * 0.5f;
                        float ptY = pt.getPosition().Y + pt.getHeight() * 0.5f;
                        float soX = soldier.getPosition().X + soldier.getWidth() * 0.5f;
                        float soY = soldier.getPosition().Y + soldier.getHeight() * 0.5f;

                        if (soldier.getSide() == SIDE_ENEMY && pt.getSide() == SIDE_PLAYER)
                        {
                            //ptX += pt.getWidth() * 0.5f;
                            //soX -= soldier.getWidth() * 0.5f;
                            if (
                                (ptX < soX && soX - ptX <= Soldier.WIDTH * range) &&
                                (Math.Abs(ptY - soY) <= Soldier.HEIGHT * 0.5f)
                               )
                            {
                                soldier.setReactionDest(ptX + Soldier.WIDTH * range);
                                return true;
                            }
                        }
                        else if (soldier.getSide() == SIDE_PLAYER && pt.getSide() == SIDE_ENEMY)
                        {
                            //ptX -= pt.getWidth() * 0.5f;
                            //soX += soldier.getWidth() * 0.5f;
                            if (
                                (ptX > soX + soldier.getWidth() && ptX - (soX + soldier.getWidth()) <= Soldier.WIDTH * range) &&
                                (Math.Abs(ptY - soY) <= Soldier.HEIGHT * 0.5f)
                               )
                            {
                                soldier.setReactionDest(ptX - soldier.getWidth() - Soldier.WIDTH * range);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal bool findSoldier(Soldier soldier, float range, float spread)
        {
            foreach (ScreenObject pt in _screenObjects)
            {
                if (pt is Soldier && pt.getSide() != soldier.getSide())
                {
                    if (((Soldier)pt).getState() != Soldier.STATE_DEAD && ((Soldier)pt).getState() != Soldier.STATE_DYING)
                    {
                        // figure out the center of the pike tip and the center of the man
                        float ptX = pt.getPosition().X + pt.getWidth() * 0.5f;
                        float ptY = pt.getPosition().Y + pt.getHeight() * 0.5f;
                        float soX = soldier.getPosition().X; //+ soldier.getWidth() * 0.5f;
                        float soY = soldier.getPosition().Y + soldier.getHeight() * 0.5f;

                        if (soldier.getSide() == SIDE_ENEMY && pt.getSide() == SIDE_PLAYER)
                        {
                            bool bool1 = ptX < soX;
                            bool bool2 = soX - (ptX + pt.getWidth()) <= Soldier.WIDTH * range;
                            bool bool3 = Math.Abs(ptY - soY) <= Soldier.WIDTH * spread;
                            if (
                                (bool1 && bool2) &&
                                (bool3)
                               )
                            {
                                //soldier.react(ptX + Soldier.WIDTH * range);
                                return true;
                            }
                        }
                        else if (soldier.getSide() == SIDE_PLAYER && pt.getSide() == SIDE_ENEMY)
                        {
                            if (
                                (ptX > soX + soldier.getWidth() && ptX - (soX + soldier.getWidth()) <= Soldier.WIDTH * range) &&
                                (Math.Abs(ptY - soY) <= Soldier.WIDTH * spread)
                               )
                            {
                                //soldier.react(ptX - soldier.getWidth() - Soldier.WIDTH * range);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal bool findShot(Soldier soldier, float range)
        {
            foreach (ScreenObject pt in _screenObjects)
            {
                if (pt is Shot && !(pt is Pavise) && pt.getSide() != soldier.getSide())
                {
                    // figure out the center of the pike tip and the center of the man
                    float ptX = pt.getPosition().X + pt.getWidth() * 0.5f;
                    float ptY = pt.getPosition().Y + pt.getHeight() * 0.5f;
                    float soX = soldier.getPosition().X + (soldier.getWidth() * 0.5f);
                    float soY = soldier.getPosition().Y + (soldier.getHeight() * 0.5f);

                    if (soldier.getSide() == SIDE_ENEMY && pt.getSide() == SIDE_PLAYER)
                    {
                        if (
                            (ptX < soX && soX - ptX <= Soldier.WIDTH * range + 5f) &&
                            (Math.Abs(ptY - soY) <= Soldier.HEIGHT * 0.5f)
                           )
                        {
                            soldier.react(ptX + Soldier.WIDTH * range);
                            return true;
                        }
                    }
                    else if (soldier.getSide() == SIDE_PLAYER && pt.getSide() == SIDE_ENEMY)
                    {
                        if (
                            (ptX > soX && ptX - soX <= Soldier.WIDTH * range + 5f) &&
                            (Math.Abs(ptY - soY) <= Soldier.HEIGHT * 0.5f)
                           )
                        {
                            soldier.react(ptX - soldier.getWidth() - Soldier.WIDTH * range);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class DrawJob
    {
        public Sprite  sprite;
        public Vector2 position;
        public int side;
        public float drawingY;

        public DrawJob(Sprite sprite, Vector2 position, int side, float drawingY)
        {
            this.sprite = sprite;
            this.position = position;
            this.side = side;
            this.drawingY = drawingY;
        }
    }
}