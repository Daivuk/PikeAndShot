﻿using System;
using System.Xml;
using System.IO;
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
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PikeAndShot
{
    public class PikeAndShotGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static SpriteFont soldierFont;
        public static Random random = new Random();

        public LevelConstructorForm _form;

        public const int SCREENWIDTH = 1028;
        public const int SCREENHEIGHT = 768;

        public const int SCREEN_LEVELPLAY = 0;
        public const int SCREEN_FORMATIONMAKER = 1;
        public const int SCREEN_LEVELEDITOR = 2;


        public static Texture2D TERRAIN_DRY_GRASS;

        public static Texture2D PIKEMAN_FEET;
        public static Texture2D PIKEMAN_IDLE;
        public static Texture2D PIKEMAN_LOWER_LOW;
        public static Texture2D PIKEMAN_LOWER_HIGH;
        public static Texture2D PIKEMAN_RECOIL;
        public static Texture2D PIKEMAN_DEATH;
        public static Texture2D PIKEMAN1_IDLE;
        public static Texture2D PIKEMAN1_LOWER_LOW;
        public static Texture2D PIKEMAN1_LOWER_HIGH;
        public static Texture2D PIKEMAN1_RECOIL;
        public static Texture2D PIKEMAN1_DEATH;
        public static Texture2D PIKEMAN1_MELEE;
        public static Texture2D PIKEMAN1_DEFEND;
        public static Texture2D PIKEMAN2_IDLE;
        public static Texture2D PIKEMAN2_LOWER_LOW;
        public static Texture2D PIKEMAN2_LOWER_HIGH;
        public static Texture2D PIKEMAN2_RECOIL;
        public static Texture2D PIKEMAN2_DEATH;
        public static Texture2D PIKEMAN2_MELEE;
        public static Texture2D PIKEMAN2_DEFEND;
        public static Texture2D PIKEMAN_MELEE;
        public static Texture2D PIKEMAN_ROUTE;
        public static Texture2D PIKEMAN_ROUTED;

        public static Texture2D ARQUEBUSIER_FEET;
        public static Texture2D ARQUEBUSIER_IDLE;
        public static Texture2D ARQUEBUSIER_DEATH;
        public static Texture2D ARQUEBUSIER_MELEE;
        public static Texture2D ARQUEBUSIER_RELOAD;
        public static Texture2D ARQUEBUSIER_SHOOT;
        public static Texture2D ARQUEBUSIER_SMOKE;
        public static Texture2D ARQUEBUSIER_ROUTE;
        public static Texture2D ARQUEBUSIER_ROUTED;
        public static Texture2D ARQUEBUSIER_SHOT1;
        public static Texture2D ARQUEBUSIER_SHOT2;
        public static Texture2D ARQUEBUSIER_SHOT3;
        public static Texture2D ARQUEBUSIER_GROUND;

        public static Texture2D CROSSBOWMAN_IDLE;
        public static Texture2D CROSSBOWMAN_DEATH;
        public static Texture2D CROSSBOWMAN_MELEE;
        public static Texture2D CROSSBOWMAN_MELEE2;
        public static Texture2D CROSSBOWMAN_DEFEND2;
        public static Texture2D CROSSBOWMAN_RELOAD;
        public static Texture2D CROSSBOWMAN_RELOAD2;
        public static Texture2D CROSSBOWMAN_SHOOT;
        public static Texture2D CROSSBOWMAN_BOLT;
        public static Texture2D CROSSBOWMAN_BOLT2;
        public static Texture2D CROSSBOWMAN_GROUND;
        public static Texture2D CROSSBOWMAN_ROUTE;
        public static Texture2D CROSSBOWMAN_ROUTED;
        public static Texture2D CROSSBOWMAN_PAVISE;
        public static Texture2D CROSSBOWMAN_PAVISE_PLACE;
        
        public static Texture2D PLACED_PAVISE;
        public static Texture2D PAVISE_FALL;

        public static Texture2D DOPPLE_IDLE;
        public static Texture2D DOPPLE_DEATH;
        public static Texture2D DOPPLE_SWING1;
        public static Texture2D DOPPLE_RELOAD1;
        public static Texture2D DOPPLE_ROUTE;
        public static Texture2D DOPPLE_ROUTED;

        public static Texture2D SOLDIER_FEET;
        public static Texture2D SOLDIER_IDLE;
        public static Texture2D SOLDIER_DEATH;
        public static Texture2D SOLDIER_MELEE1;
        public static Texture2D SOLDIER_DEFEND1;
        public static Texture2D SOLDIER_MELEE2;
        public static Texture2D SOLDIER_DEFEND2;
        public static Texture2D SOLDIER_ROUTE;
        public static Texture2D SOLDIER_ROUTED;
        public static Texture2D SOLDIER_SHIELDBREAK;
        public static Texture2D SOLDIER_IDLENOSHIELD;
        public static Texture2D SOLDIER_BROKENSHIELD1;
        public static Texture2D SOLDIER_BROKENSHIELD2;
        public static Texture2D SOLDIER_FALL;
        public static Texture2D SOLDIER_CHARGE;
        public static Texture2D SOLDIER_CHARGENOSHIELD;

        public static Texture2D CAVALRY_HORSE_IDLE;
        public static Texture2D CAVALRY_HORSE_RUN;
        public static Texture2D CAVALRY_HORSE_HALT;
        public static Texture2D CAVALRY_HORSE_TURN;
        public static Texture2D CAVALRY_HORSE_DEATH;
        public static Texture2D CAVALRY_FALL;
        public static Texture2D CAVALRY_LEFTFOOT;
        public static Texture2D CAVALRY_RIGHTFOOT;
        public static Texture2D CAVALRY_LEFTIDLE;
        public static Texture2D CAVALRY_RIGHTIDLE;
        public static Texture2D CAVALRY_LEFTLOWER;
        public static Texture2D CAVALRY_RIGHTLOWER;
        public static Texture2D CAVALRY_LEFTRECOIL;
        public static Texture2D CAVALRY_RIGHTRECOIL;

        public static Texture2D DISMOUNTED_CAVALRY_IDLE;
        public static Texture2D DISMOUNTED_CAVALRY_DEATH;
        public static Texture2D DISMOUNTED_CAVALRY_IDLENOSHIELD;
        public static Texture2D DISMOUNTED_CAVALRY_SHIELDBREAK;
        public static Texture2D DISMOUNTED_CAVALRY_FALL;
        public static Texture2D DISMOUNTED_CAVALRY_MELEE1;
        public static Texture2D DISMOUNTED_CAVALRY_DEFEND1;

        public static Texture2D SLINGER_IDLE;
        public static Texture2D SLINGER_DEATH;
        public static Texture2D SLINGER_MELEE;
        public static Texture2D SLINGER_DEFEND;
        public static Texture2D SLINGER_RELOAD;
        public static Texture2D SLINGER_SHOOT;
        public static Texture2D SLINGER_ROCK;
        public static Texture2D GOBLIN_FEET;
        public static Texture2D SLINGER_GROUND;
        public static Texture2D SLINGER_ROUTE;
        public static Texture2D SLINGER_ROUTED;
        public static Texture2D SLINGER_RETREAT;

        public static Texture2D SKIRMISHER_IDLE;
        public static Texture2D SKIRMISHER_RELOAD;
        public static Texture2D SKIRMISHER_SHOOT;
        public static Texture2D SKIRMISHER_JAVELIN;
        public static Texture2D SKIRMISHER_GROUND;
        public static Texture2D SKIRMISHER_DEATH;
        public static Texture2D SKIRMISHER_MELEE;
        public static Texture2D SKIRMISHER_DEFEND;
        public static Texture2D SKIRMISHER_RETREAT;

        public static Texture2D BERZERKER_IDLE;
        public static Texture2D BERZERKER_DEATH;
        public static Texture2D BERZERKER_MELEE1;
        public static Texture2D BERZERKER_DEFEND1;
        public static Texture2D BERZERKER_MELEE2;
        public static Texture2D BERZERKER_DEFEND2;
        public static Texture2D BERZERKER_ROUTE;
        public static Texture2D BERZERKER_ROUTED;
        public static Texture2D BERZERKER_IDLENOSHIELD;
        public static Texture2D BERZERKER_SHIELDBREAK;
        public static Texture2D BERZERKER_FALL;
        public static Texture2D BERZERKER_CHARGE;
        public static Texture2D BERZERKER_CHARGENOSHIELD;

        public static Texture2D BERZERKER2_IDLE;
        public static Texture2D BERZERKER2_DEATH;
        public static Texture2D BERZERKER2_MELEE1;
        public static Texture2D BERZERKER2_DEFEND1;
        public static Texture2D BERZERKER2_MELEE2;
        public static Texture2D BERZERKER2_DEFEND2;
        public static Texture2D BERZERKER2_ROUTE;
        public static Texture2D BERZERKER2_ROUTED;
        public static Texture2D BERZERKER2_IDLENOSHIELD;
        public static Texture2D BERZERKER2_SHIELDBREAK;
        public static Texture2D BERZERKER2_FALL;
        public static Texture2D BERZERKER2_CHARGE;
        public static Texture2D BERZERKER2_CHARGENOSHIELD;

        public static Texture2D BRIGAND1_IDLE;
        public static Texture2D BRIGAND1_DEATH;
        public static Texture2D BRIGAND1_MELEE1;
        public static Texture2D BRIGAND1_DEFEND1;
        public static Texture2D BRIGAND1_CHARGE;

        public static Texture2D BRIGAND2_IDLE;
        public static Texture2D BRIGAND2_DEATH;
        public static Texture2D BRIGAND2_MELEE1;
        public static Texture2D BRIGAND2_DEFEND1;
        public static Texture2D BRIGAND2_CHARGE;

        public static Texture2D BROWN_FEET;

        public static List<Texture2D> ROAD_TERRAIN;


        //Utility Graphics
        public static Texture2D DOT;
        public static Texture2D SWORD_POINTER;

        private ArrayList _gameScreens;
        private BattleScreen _currScreen;

        public PikeAndShotGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREENWIDTH;
            graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            graphics.PreferMultiSampling = false;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            soldierFont = Content.Load<SpriteFont>("SpriteFont1");

            //TERRAIN_DRY_GRASS = Content.Load<Texture2D>(@"dry_grass");
            ROAD_TERRAIN = new List<Texture2D>(11);

            for(int i = 0; i < 11; i++)
                ROAD_TERRAIN.Add(Content.Load<Texture2D>(@"roadTerrain" + i));

            SOLDIER_FEET = Content.Load<Texture2D>(@"soldier_feet");
            SOLDIER_IDLE = Content.Load<Texture2D>(@"soldier_idle");
            SOLDIER_DEATH = Content.Load<Texture2D>(@"soldier_death");
            SOLDIER_MELEE1 = Content.Load<Texture2D>(@"soldier_melee1");
            SOLDIER_DEFEND1 = Content.Load<Texture2D>(@"soldier_defend1");
            SOLDIER_MELEE2 = Content.Load<Texture2D>(@"soldier_melee2");
            SOLDIER_DEFEND2 = Content.Load<Texture2D>(@"soldier_defend2");
            SOLDIER_SHIELDBREAK = Content.Load<Texture2D>(@"soldier_shieldbreak");
            SOLDIER_FALL = Content.Load<Texture2D>(@"soldier_fall");
            SOLDIER_BROKENSHIELD1 = Content.Load<Texture2D>(@"brokenshield1");
            SOLDIER_BROKENSHIELD2 = Content.Load<Texture2D>(@"brokenshield2");
            SOLDIER_IDLENOSHIELD = Content.Load<Texture2D>(@"soldier_idlenoshield");
            SOLDIER_ROUTE = Content.Load<Texture2D>(@"soldier_route");
            SOLDIER_ROUTED = Content.Load<Texture2D>(@"soldier_routed");
            SOLDIER_CHARGENOSHIELD = Content.Load<Texture2D>(@"soldier_chargenoshield");
            SOLDIER_CHARGE = Content.Load<Texture2D>(@"soldier_charge");

            CAVALRY_HORSE_IDLE = Content.Load<Texture2D>(@"horse_idle");
            CAVALRY_HORSE_RUN = Content.Load<Texture2D>(@"horse_run");
            CAVALRY_HORSE_HALT = Content.Load<Texture2D>(@"horse_halt");
            CAVALRY_HORSE_TURN = Content.Load<Texture2D>(@"horse_turn");
            CAVALRY_HORSE_DEATH = Content.Load<Texture2D>(@"horse_death");
            CAVALRY_LEFTFOOT = Content.Load<Texture2D>(@"cavalry_leftfoot");
            CAVALRY_RIGHTFOOT = Content.Load<Texture2D>(@"cavalry_rightfoot");
            CAVALRY_LEFTIDLE = Content.Load<Texture2D>(@"cavalry_leftidle");
            CAVALRY_RIGHTIDLE = Content.Load<Texture2D>(@"cavalry_rightidle");
            CAVALRY_LEFTLOWER = Content.Load<Texture2D>(@"cavalry_leftlower");
            CAVALRY_RIGHTLOWER = Content.Load<Texture2D>(@"cavalry_rightlower");
            CAVALRY_LEFTRECOIL = Content.Load<Texture2D>(@"cavalry_leftrecoil");
            CAVALRY_RIGHTRECOIL = Content.Load<Texture2D>(@"cavalry_rightrecoil");

            CAVALRY_FALL = Content.Load<Texture2D>(@"cavalry_fall");
            DISMOUNTED_CAVALRY_IDLE = Content.Load<Texture2D>(@"dismountedcavalry_idle");
            DISMOUNTED_CAVALRY_DEATH = Content.Load<Texture2D>(@"dismountedcavalry_death");
            DISMOUNTED_CAVALRY_IDLENOSHIELD = Content.Load<Texture2D>(@"dismountedcavalry_idlenoshield");
            DISMOUNTED_CAVALRY_SHIELDBREAK = Content.Load<Texture2D>(@"dismountedcavalry_shieldbreak");
            DISMOUNTED_CAVALRY_FALL = Content.Load<Texture2D>(@"dismountedcavalry_fall");
            DISMOUNTED_CAVALRY_MELEE1 = Content.Load<Texture2D>(@"dismountedcavalry_melee1");
            DISMOUNTED_CAVALRY_DEFEND1 = Content.Load<Texture2D>(@"dismountedcavalry_defend1");

            PIKEMAN_FEET = Content.Load<Texture2D>(@"pikeman_feet");
            PIKEMAN_IDLE = Content.Load<Texture2D>(@"pikeman_idle");
            PIKEMAN_LOWER_LOW = Content.Load<Texture2D>(@"pikeman_lower_low");
            PIKEMAN_LOWER_HIGH = Content.Load<Texture2D>(@"pikeman_lower_high");
            PIKEMAN_RECOIL = Content.Load<Texture2D>(@"pikeman_recoil");
            PIKEMAN_DEATH = Content.Load<Texture2D>(@"pikeman_death");
            PIKEMAN1_IDLE = Content.Load<Texture2D>(@"pikeman1_idle");
            PIKEMAN1_LOWER_LOW = Content.Load<Texture2D>(@"pikeman1_lower_low");
            PIKEMAN1_LOWER_HIGH = Content.Load<Texture2D>(@"pikeman1_lower_high");
            PIKEMAN1_RECOIL = Content.Load<Texture2D>(@"pikeman1_recoil");
            PIKEMAN1_DEATH = Content.Load<Texture2D>(@"pikeman1_death");
            PIKEMAN1_MELEE = Content.Load<Texture2D>(@"pikeman1_melee1");
            PIKEMAN1_DEFEND = Content.Load<Texture2D>(@"pikeman1_defend1");
            PIKEMAN2_IDLE = Content.Load<Texture2D>(@"pikeman2_idle");
            PIKEMAN2_LOWER_LOW = Content.Load<Texture2D>(@"pikeman2_lower_low");
            PIKEMAN2_LOWER_HIGH = Content.Load<Texture2D>(@"pikeman2_lower_high");
            PIKEMAN2_RECOIL = Content.Load<Texture2D>(@"pikeman2_recoil");
            PIKEMAN2_DEATH = Content.Load<Texture2D>(@"pikeman2_death");
            PIKEMAN2_MELEE = Content.Load<Texture2D>(@"pikeman2_melee1");
            PIKEMAN2_DEFEND = Content.Load<Texture2D>(@"pikeman2_defend1");
            PIKEMAN_MELEE = Content.Load<Texture2D>(@"pikeman_melee");
            PIKEMAN_ROUTE = Content.Load<Texture2D>(@"pikeman_route");
            PIKEMAN_ROUTED = Content.Load<Texture2D>(@"pikeman_routed");

            ARQUEBUSIER_FEET = Content.Load<Texture2D>(@"arquebusier_feet");
            ARQUEBUSIER_IDLE = Content.Load<Texture2D>(@"arquebusier_idle");
            ARQUEBUSIER_RELOAD = Content.Load<Texture2D>(@"arquebusier_reload");
            ARQUEBUSIER_SHOOT = Content.Load<Texture2D>(@"arquebusier_shoot");
            ARQUEBUSIER_SMOKE = Content.Load<Texture2D>(@"arquebusier_smoke");
            ARQUEBUSIER_DEATH = Content.Load<Texture2D>(@"arquebusier_death");
            ARQUEBUSIER_MELEE = Content.Load<Texture2D>(@"arquebusier_melee");
            ARQUEBUSIER_ROUTE = Content.Load<Texture2D>(@"arquebusier_route");
            ARQUEBUSIER_ROUTED = Content.Load<Texture2D>(@"arquebusier_routed");
            ARQUEBUSIER_SHOT1 = Content.Load<Texture2D>(@"shot1");
            ARQUEBUSIER_SHOT2 = Content.Load<Texture2D>(@"shot2");
            ARQUEBUSIER_SHOT3 = Content.Load<Texture2D>(@"shot3");
            ARQUEBUSIER_GROUND = Content.Load<Texture2D>(@"arquebusier_ground");

            CROSSBOWMAN_IDLE = Content.Load<Texture2D>(@"crossbowman_idle");
            CROSSBOWMAN_RELOAD = Content.Load<Texture2D>(@"crossbowman_reload");
            CROSSBOWMAN_RELOAD2 = Content.Load<Texture2D>(@"crossbowman_reload2");
            CROSSBOWMAN_SHOOT = Content.Load<Texture2D>(@"crossbowman_shoot");
            CROSSBOWMAN_DEATH = Content.Load<Texture2D>(@"crossbowman_death");
            CROSSBOWMAN_MELEE = Content.Load<Texture2D>(@"crossbowman_melee");
            CROSSBOWMAN_MELEE2 = Content.Load<Texture2D>(@"crossbowman_melee2");
            CROSSBOWMAN_DEFEND2 = Content.Load<Texture2D>(@"crossbowman_defend2");
            CROSSBOWMAN_BOLT = Content.Load<Texture2D>(@"crossbowman_bolt");
            CROSSBOWMAN_BOLT2 = Content.Load<Texture2D>(@"crossbowman_bolt2");
            CROSSBOWMAN_GROUND = Content.Load<Texture2D>(@"crossbowman_ground");
            CROSSBOWMAN_ROUTE = Content.Load<Texture2D>(@"crossbowman_route");
            CROSSBOWMAN_ROUTED = Content.Load<Texture2D>(@"crossbowman_routed");
            CROSSBOWMAN_PAVISE = Content.Load<Texture2D>(@"crossbowman_pavise_idle");
            CROSSBOWMAN_PAVISE_PLACE = Content.Load<Texture2D>(@"crossbowman_pavise_place");
            
            PLACED_PAVISE = Content.Load<Texture2D>(@"placed_pavise");
            PAVISE_FALL = Content.Load<Texture2D>(@"pavise_fall");

            DOPPLE_DEATH = Content.Load<Texture2D>(@"dopple_death");
            DOPPLE_IDLE = Content.Load<Texture2D>(@"dopple_idle");
            DOPPLE_SWING1 = Content.Load<Texture2D>(@"dopple_swing1");
            DOPPLE_RELOAD1 = Content.Load<Texture2D>(@"dopple_reload1");
            DOPPLE_ROUTE = Content.Load<Texture2D>(@"dopple_route");
            DOPPLE_ROUTED = Content.Load<Texture2D>(@"dopple_routed");

            SLINGER_IDLE = Content.Load<Texture2D>(@"slinger_idle");
            SLINGER_DEATH = Content.Load<Texture2D>(@"slinger_death");
            SLINGER_MELEE = Content.Load<Texture2D>(@"slinger_melee2");
            SLINGER_DEFEND = Content.Load<Texture2D>(@"slinger_defend2");
            SLINGER_RELOAD = Content.Load<Texture2D>(@"slinger_reload");
            SLINGER_ROCK = Content.Load<Texture2D>(@"slinger_rock");
            SLINGER_SHOOT = Content.Load<Texture2D>(@"slinger_shoot");
            SLINGER_ROUTE = Content.Load<Texture2D>(@"slinger_route");
            SLINGER_ROUTED = Content.Load<Texture2D>(@"slinger_routed");
            SLINGER_RETREAT = Content.Load<Texture2D>(@"slinger_retreat");

            SKIRMISHER_IDLE = Content.Load<Texture2D>(@"skirmisher_idle");
            SKIRMISHER_RELOAD = Content.Load<Texture2D>(@"skirmisher_reload");
            SKIRMISHER_SHOOT = Content.Load<Texture2D>(@"skirmisher_shoot");
            SKIRMISHER_JAVELIN = Content.Load<Texture2D>(@"skirmisher_javelin");
            SKIRMISHER_GROUND = Content.Load<Texture2D>(@"skirmisher_ground");
            SKIRMISHER_DEATH = Content.Load<Texture2D>(@"skirmisher_death");
            SKIRMISHER_MELEE = Content.Load<Texture2D>(@"skirmisher_melee2");
            SKIRMISHER_DEFEND = Content.Load<Texture2D>(@"skirmisher_defend2");
            SKIRMISHER_RETREAT = Content.Load<Texture2D>(@"skirmisher_retreat");

            BERZERKER_IDLE = Content.Load<Texture2D>(@"gobraider1_idle");
            BERZERKER_DEATH = Content.Load<Texture2D>(@"gobraider1_death");
            BERZERKER_MELEE1 = Content.Load<Texture2D>(@"gobraider1_melee1");
            BERZERKER_DEFEND1 = Content.Load<Texture2D>(@"gobraider1_defend1");
            BERZERKER_MELEE2 = Content.Load<Texture2D>(@"gobraider1_melee2");
            BERZERKER_DEFEND2 = Content.Load<Texture2D>(@"gobraider1_defend2");
            BERZERKER_ROUTE = Content.Load<Texture2D>(@"beserker_route");
            BERZERKER_ROUTED = Content.Load<Texture2D>(@"beserker_routed");
            BERZERKER_IDLENOSHIELD = Content.Load<Texture2D>(@"gobraider1_idlenoshield");
            BERZERKER_SHIELDBREAK = Content.Load<Texture2D>(@"gobraider1_shieldbreak");
            BERZERKER_FALL = Content.Load<Texture2D>(@"gobraider1_fall");
            BERZERKER_CHARGENOSHIELD = Content.Load<Texture2D>(@"gobraider1_chargenoshield");
            BERZERKER_CHARGE = Content.Load<Texture2D>(@"gobraider1_charge");

            BERZERKER2_IDLE = Content.Load<Texture2D>(@"gobraider2_idle");
            BERZERKER2_DEATH = Content.Load<Texture2D>(@"gobraider2_death");
            BERZERKER2_MELEE1 = Content.Load<Texture2D>(@"gobraider2_melee1");
            BERZERKER2_DEFEND1 = Content.Load<Texture2D>(@"gobraider2_defend1");
            BERZERKER2_MELEE2 = Content.Load<Texture2D>(@"gobraider2_melee2");
            BERZERKER2_DEFEND2 = Content.Load<Texture2D>(@"gobraider2_defend2");
            BERZERKER2_ROUTE = Content.Load<Texture2D>(@"beserker_route");
            BERZERKER2_ROUTED = Content.Load<Texture2D>(@"beserker_routed");
            BERZERKER2_IDLENOSHIELD = Content.Load<Texture2D>(@"gobraider2_idlenoshield");
            BERZERKER2_SHIELDBREAK = Content.Load<Texture2D>(@"gobraider2_shieldbreak");
            BERZERKER2_FALL = Content.Load<Texture2D>(@"gobraider2_fall");
            BERZERKER2_CHARGENOSHIELD = Content.Load<Texture2D>(@"gobraider2_chargenoshield");
            BERZERKER2_CHARGE = Content.Load<Texture2D>(@"gobraider2_charge");

            BRIGAND1_IDLE = Content.Load<Texture2D>(@"berzerker_idlenoshield");
            BRIGAND1_DEATH = Content.Load<Texture2D>(@"bezerker_death");
            BRIGAND1_MELEE1 = Content.Load<Texture2D>(@"berzerker_melee2");
            BRIGAND1_DEFEND1 = Content.Load<Texture2D>(@"berzerker_defend2");
            BRIGAND1_CHARGE = Content.Load<Texture2D>(@"berzerker_chargenoshield");

            BRIGAND2_IDLE = Content.Load<Texture2D>(@"brigand_idle");
            BRIGAND2_DEATH = Content.Load<Texture2D>(@"brigand_death");
            BRIGAND2_MELEE1 = Content.Load<Texture2D>(@"brigand_melee2");
            BRIGAND2_DEFEND1 = Content.Load<Texture2D>(@"brigand_defend2");
            BRIGAND2_CHARGE = Content.Load<Texture2D>(@"brigand_charge");

            GOBLIN_FEET = Content.Load<Texture2D>(@"goblin_feet");
            BROWN_FEET = Content.Load<Texture2D>(@"brown_feet");
            SLINGER_GROUND = Content.Load<Texture2D>(@"slinger_ground");

            DOT = Content.Load<Texture2D>(@"dot");
            SWORD_POINTER = Content.Load<Texture2D>(@"sword_pointer");

            _gameScreens = new ArrayList(3);

            // MAKE LEVEL
            
            XmlReaderSettings settings = new XmlReaderSettings();
            FileStream stream = new FileStream("Content\\Benz_Level1.xml", FileMode.Open);
            LevelPipeline.LevelContent levelContent;
            using (XmlReader xmlReader = XmlReader.Create(stream, settings))
            {
                levelContent = IntermediateSerializer.Deserialize<LevelPipeline.LevelContent>(xmlReader, null);
            }
            stream.Close();
            List<Level> levels = new List<Level>(32);
            levels.Add(new Level(levelContent));
            
            //levels.Add(Content.Load<Level>(@"gobwall"));
            //levels.Add(Content.Load<Level>(@"GoblinLevel"));
            //levels.Add(Content.Load<Level>(@"Benz_Level1"));
            //levels.Add(Content.Load<Level>(@"gob_level"));
            //levels.Add(Content.Load<Level>(@"HorseAttack"));
            
            // PLAY LEVEL
            _currScreen = new LevelScreen(this, levels.ElementAt(0));
            _gameScreens.Add(_currScreen);

            // MAKE FORMATION
            _gameScreens.Add(new FormationEditorScreen(this));
            _form = new LevelConstructorForm(levels);
            _gameScreens.Add( new LevelEditorScreen(this, _form));
            _form.addFormListener((FormListener)_gameScreens[SCREEN_LEVELEDITOR]);
            _form.addFormListener((LevelScreen)_currScreen);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currScreen != null)
            {
                _currScreen.update(gameTime);
            }
            base.Update(gameTime);
        }

        public void setScreen(int screenIndex)
        {
            if (screenIndex >= 0 && screenIndex <= SCREEN_LEVELEDITOR)
                _currScreen = (BattleScreen)_gameScreens[screenIndex];

            if (screenIndex == SCREEN_LEVELEDITOR)
                _form.Show();
            else
                _form.Hide();

            if (screenIndex == SCREEN_LEVELPLAY)
                ((LevelScreen)_currScreen).restart();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //get rid of blurry sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            graphics.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;

            if (_currScreen != null)
            {
                _currScreen.draw(gameTime, spriteBatch);
            }
            base.Draw(gameTime);

            spriteBatch.End();
        }

        internal static SpriteFont getSpriteFont()
        {
            return soldierFont;
        }

        internal static Texture2D getDotTexture()
        {
            return DOT;
        }

        // helper function to randomly make a number positive or negative
        internal static int getRandPlusMinus(int in_number)
        {
            int number = random.Next(in_number);

            if (random.Next(2) == 0)
                return number;
            else
                return number * -1;
        }
    }
}