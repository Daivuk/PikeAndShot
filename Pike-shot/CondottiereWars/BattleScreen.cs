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
        protected ArrayList _moreDeadThings;
        public ArrayList _deadThings;
        public ArrayList unlooseSoldiers;
        protected ArrayList _newThings;
        protected ArrayList _deadFormations;
        protected ArrayList _looseSoldiers;
        protected ArrayList _screenObjects;
        protected ArrayList _screenObjectsToAdd;
        protected ArrayList _screenColliders;
        protected ArrayList _screenNonColliders;
        protected ArrayList _screenAnimations;
        protected ArrayList _screenAnimationsToAdd;
        public ArrayList _enemyFormations;
        protected ArrayList _terrain;
        protected ArrayList _drawJobs;
        protected ArrayList _enemyFormationsToAdd;

        public PikeAndShotGame _game;

        protected Formation _formation;
        protected KeyboardState keyboardState;
        protected KeyboardState previousKeyboardState;
        protected GamePadState gamePadState;
        protected GamePadState previousGamePadState;

        protected double _elapsedTime;
        protected Vector2 _mapOffset;
        public bool playerInPlay;

        public BattleScreen(PikeAndShotGame game)
        {
            _game = game;
            playerInPlay = true;

            //shot and clean up arrays
            _shots = new ArrayList(40);
            _deadThings = new ArrayList(40);
            _moreDeadThings = new ArrayList(2);
            _newThings = new ArrayList(40);
            _deadFormations = new ArrayList(20);

            //arrays for collisions
            _screenObjects = new ArrayList(40);
            _screenObjectsToAdd = new ArrayList(40);
            _screenColliders = new ArrayList(40);
            _screenNonColliders = new ArrayList(20);
            _screenAnimations = new ArrayList(40);
            _screenAnimationsToAdd = new ArrayList(40);
            _enemyFormationsToAdd = new ArrayList(3);

            _enemyFormations = new ArrayList(25);
            _looseSoldiers = new ArrayList(40);
            unlooseSoldiers = new ArrayList(5);
            
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

            foreach (ScreenAnimation screeny in this._screenAnimationsToAdd)
            {
                _screenAnimations.Add(screeny);
            }
            _screenAnimationsToAdd.Clear();

            foreach (Formation f in _enemyFormations)
            {
                f.update(gameTime.ElapsedGameTime);
                if (f.getSide() == SIDE_ENEMY && (f.getPosition().X < (-1 * f.getTotalRows() * Soldier.WIDTH) + _mapOffset.X || f.getTotalRows() == 0))
                {
                    if(!f.hasSoldierOnScreen() && this is LevelScreen)
                        _deadFormations.Add(f);
                }
            }

            foreach (Formation f in _enemyFormationsToAdd)
                _enemyFormations.Add(f);

            _enemyFormationsToAdd.Clear();
            
            //checking for empty or off screen formations
            foreach (Formation f in _deadFormations)
            {
                _enemyFormations.Remove(f);
                foreach (Soldier sold in f.getSoldiers())
                    _deadThings.Add(sold);
            }
            _deadFormations.Clear();
            
            checkCollisions(gameTime.ElapsedGameTime);

            foreach (Soldier sold in _looseSoldiers)
            {
                sold.update(gameTime.ElapsedGameTime);

                if (sold.getSide() == BattleScreen.SIDE_PLAYER && sold.getPosition().X > PikeAndShotGame.SCREENWIDTH + getMapOffset().X + 2f * Soldier.WIDTH)
                    sold.setState(Soldier.STATE_DEAD);
                else if (sold.getSide() == BattleScreen.SIDE_ENEMY && sold.getPosition().X < -210 + getMapOffset().X)
                    sold.setState(Soldier.STATE_DEAD);

                if (sold.isDead())
                {
                    _deadThings.Add(sold);
                }
            }
            foreach (Soldier s in unlooseSoldiers)
            {
                _looseSoldiers.Remove(s);
            }
            unlooseSoldiers.Clear();

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
            foreach (ScreenObject obj in _moreDeadThings)
                _deadThings.Add(obj);

            _moreDeadThings.Clear();

            foreach (ScreenObject obj in _deadThings)
            {
                if (obj is Shot)
                    _shots.Remove(obj);
                else if (obj is Soldier)
                {
                    _looseSoldiers.Remove(obj);
                }
                else if (obj is Terrain)
                {
                    _terrain.Remove(obj);
                    _terrain.Add(new Terrain(this, PikeAndShotGame.ROAD_TERRAIN[PikeAndShotGame.random.Next(7)], SIDE_PLAYER, PikeAndShotGame.SCREENWIDTH + getMapOffset().X, PikeAndShotGame.random.Next(PikeAndShotGame.SCREENHEIGHT)));
                }

                if (obj is WeaponSwing)
                    ((WeaponSwing)obj).getHeight();

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

            foreach (Object ani in _deadThings)
            {
                if (ani is ScreenAnimation)
                    _screenAnimations.Remove(ani);
                else if (ani is ScreenObject)
                    _screenObjects.Remove(ani);
            }

            _deadThings.Clear();
        }

        public void addScreenObject(ScreenObject so)
        {
            _screenObjectsToAdd.Add(so);
            //_screenObjects.Add(so);
        }

        public ArrayList getScreenObjects()
        {
            return _screenObjects;
        }

        public Formation getPlayerFormation()
        {
            return _formation;
        }

        public Vector2 getMapOffset()
        {
            return _mapOffset;
        }

        protected void checkCollisions(TimeSpan timeSpan)
        {
            int x = 0;

            float soX, soY, soWidth, soHeight;
            float coX, coY, coWidth, coHeight;
            
            bool collision = true;
            bool oneCollision = false;

            _screenColliders.Clear();
            _screenNonColliders.Clear();

            // pour all of the screen objects into the list of objects to check for collisions against to start with
            foreach (ScreenObject so in _screenObjects)
            {
                if (so is Wolf)
                {
                    if (so.getState() != Wolf.STATE_SPOOKED && so.getState() != Wolf.STATE_HOWLING && so.getState() != Wolf.STATE_FLEE && (so.getState() != Wolf.STATE_TURNING || !((Wolf)so).flee))
                        _screenColliders.Add(so);
                    else
                        _screenNonColliders.Add(so);
                }
                else if (so is Colmillos)
                {
                    if (so.getState() == Soldier.STATE_DYING || so.getState() == Colmillos.STATE_EATEN || so.getState() == Colmillos.STATE_RISE || ((ColmillosFormation)((Colmillos)so).myFormation).attacked)
                    {
                        _screenNonColliders.Add(so);
                    }
                    else
                        _screenColliders.Add(so);
                }
                else if (so.getState() != ScreenObject.STATE_DEAD && so.getState() != ScreenObject.STATE_DYING && (so.getSide() == SIDE_ENEMY || playerInPlay))
                    _screenColliders.Add(so);
                else
                    _screenNonColliders.Add(so);
            }
            
            // Now for every object see if it hit any of the colliders
            // screenobjects that didn't hit anything can be removed from the list of coliders so they aren't checked repeatedly for no reason
            foreach (ScreenObject so in _screenObjects)
            {
                if (so is WeaponSwing)
                    x++;
                if (!_screenNonColliders.Contains(so))
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
                    else if (so is Wolf)
                    {
                        soX = so.getPosition().X;
                        soY = so.getPosition().Y - 5;
                        soWidth = so.getWidth();
                        soHeight = so.getHeight() + 5;
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
                            if (co is Pavise || co is CrossbowmanPavise && so is Shot)
                            {
                                coX = co.getPosition().X;
                                coY = co.getPosition().Y - 10;
                                coWidth = co.getWidth();
                                coHeight = co.getHeight() + 10;
                            }
                            else if (co is Wolf)
                            {
                                coX = co.getPosition().X;
                                coY = co.getPosition().Y - 5;
                                coWidth = co.getWidth();
                                coHeight = co.getHeight() + 5;
                            }
                            else
                            {
                                coX = co.getPosition().X;
                                coY = co.getPosition().Y;
                                coWidth = co.getWidth();
                                coHeight = co.getHeight();
                            }

                            if (so is CrossbowmanPavise && co is Shot)
                            {
                                soY = so.getPosition().Y - 10;
                                soHeight = so.getHeight() + 10;
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
                                if (so is WeaponAttack || co is WeaponAttack)
                                    Console.WriteLine("oh");
                                so.collide(co, timeSpan);
                                oneCollision = true;
                                co.collide(so, timeSpan);
                            }
                        }
                    }
                    if (!oneCollision)
                        _screenColliders.Remove(so);
                }
            }
        }

        public void checkNonFatalCollision(ScreenObject so, TimeSpan timeSpan)
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
                            so.collide(co, timeSpan);
                            co.collide(so, timeSpan);
                        }
                    }
                }
            }
            
        }

        public void removeScreenObject(ScreenObject so)
        {
            _moreDeadThings.Add(so);
            //_screenObjects.Remove(so);
        }

        public void removeEnemyFormation(Formation enemyFormation)
        {
            _enemyFormations.Remove(enemyFormation);
        }

        protected virtual void getInput(TimeSpan timeSpan)
        {
            // check for screen change
            
            if (keyboardState.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _game.setScreen(PikeAndShotGame.SCREEN_LEVELPLAY);
            }
            if (PikeAndShotGame.DEBUG)
            {
                if (keyboardState.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    _game.setScreen(PikeAndShotGame.SCREEN_FORMATIONMAKER);
                }
                else if (keyboardState.IsKeyDown(Keys.D3) && previousKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    _game.setScreen(PikeAndShotGame.SCREEN_LEVELEDITOR);
                }
            }
            if (keyboardState.IsKeyDown(Keys.D0) && previousKeyboardState.IsKeyUp(Keys.D0) && keyboardState.IsKeyDown(Keys.LeftControl))
            {
                _game.fullScreen();
            }
            else if (keyboardState.IsKeyDown(Keys.D9) && previousKeyboardState.IsKeyUp(Keys.D9) && keyboardState.IsKeyDown(Keys.LeftControl))
            {
                PikeAndShotGame.useShaders = !PikeAndShotGame.useShaders;
            }
        }

        protected float getScrollAdjustSpeed()
        {
            float diff = _formation.getCenter().X - (PikeAndShotGame.SCREENWIDTH * BattleScreen.SCROLLPOINT + _mapOffset.X);

            if (diff > 100f)
                return 0.2f;
            else
                return 0.2f * diff / 100f;

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
                if(dj.flashAmount>0)
                    dj.sprite.draw(spriteBatch, dj.position, dj.side, dj.flashAmount);
                else if (dj.color != Color.Black)
                    dj.sprite.draw(spriteBatch, dj.position, dj.side, (float)gameTime.TotalGameTime.TotalMilliseconds, dj.flickerTime, dj.color);
                else if (dj.flickerTime > 0)
                    dj.sprite.draw(spriteBatch, dj.position, dj.side, (float)gameTime.TotalGameTime.TotalMilliseconds, dj.flickerTime);
                else if (dj.sprite.hasEffect())
                    dj.sprite.draw(spriteBatch, dj.position, dj.side, gameTime.ElapsedGameTime);
                else
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

        public void removeShot(Shot shot)
        {
            _shots.Remove(shot);
            removeScreenObject(shot);
        }

        public ArrayList getShots()
        {
            return _shots;
        }

        internal void addLooseSoldier(Soldier sold)
        {
            if (!_looseSoldiers.Contains(sold))
            {
                _looseSoldiers.Add(sold);
                sold.myFormation = null;
            }
        }

        internal void addLooseSoldierNext(Soldier sold)
        {
            if (!_newThings.Contains(sold))
            {
                _newThings.Add(sold);
                sold.myFormation = null;
            }
        }

        internal ArrayList getLooseSoldiers()
        {
            return _looseSoldiers;
        }

        internal void removeLooseSoldier(Soldier sold)
        {
            if (_looseSoldiers.Contains(sold))
                _looseSoldiers.Remove(sold);
        }

        internal void addAnimation(ScreenAnimation screenAnimation)
        {
            this._screenAnimationsToAdd.Add(screenAnimation);
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
                    if(((PikeTip)pt).getSoldierState() != Pikeman.STATE_RECOILING || soldier.getState() == Targeteer.STATE_DEFEND || soldier.getState() == Colmillos.STATE_ATTACK)
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
                                (ptX > soX && ptX - (soX + soldier.getWidth()) <= Soldier.WIDTH * range) &&
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
        public float flashAmount;
        public float flickerTime;
        public Color color;

        public DrawJob(Sprite sprite, Vector2 position, int side, float drawingY)
        {
            this.sprite = sprite;
            this.position = position;
            this.side = side;
            this.drawingY = drawingY;
            flashAmount = 0;
            flickerTime = 0;
            color = Color.Black;
        }

        public DrawJob(Sprite sprite, Vector2 position, int side, float drawingY, float flashAmount) :
            this(sprite, position, side, drawingY)
        {
            this.flashAmount = flashAmount;
        }

        public DrawJob(Sprite sprite, Vector2 position, int side, float drawingY, bool flickering, float flickerTime) :
            this(sprite, position, side, drawingY)
        {
            this.flickerTime = flickerTime;
        }

        public DrawJob(Sprite sprite, Vector2 position, int side, float drawingY, bool flickering, float flickerTime, Color color) :
            this(sprite,position,side,drawingY,flickering,flickerTime)
        {
            this.color = color;
        }
    }
}