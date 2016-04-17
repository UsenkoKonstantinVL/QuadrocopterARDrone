using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Fuzzy;

namespace KukaForm
{
    class FuzzyLogicCrossObstacles
    {
        InferenceSystem IS;

        public FuzzyLogicCrossObstacles()
        {
            InitializeFuzzy();
        }

        void InitializeFuzzy()
        {
            FuzzySet fsNear = new FuzzySet("mf1", new TrapezoidalFunction(0, 1f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsFar = new FuzzySet("mf2", new TrapezoidalFunction(0.5f, 1f, 1.5f));
            FuzzySet fsB = new FuzzySet("mf3", new TrapezoidalFunction(0.5f, 2f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsUngL = new FuzzySet("u1", new TrapezoidalFunction(-30, 0f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsUng = new FuzzySet("u2", new TrapezoidalFunction(-30f, 0f, 30f));
            FuzzySet fsUngR = new FuzzySet("u3", new TrapezoidalFunction(0f, 30f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsSpeedAngL = new FuzzySet("saL", new TrapezoidalFunction(-1.5f, 1f, -0.5f));
            FuzzySet fsSpeedAng = new FuzzySet("sa", new TrapezoidalFunction(-0.5f, 0f, 0.5f));
            FuzzySet fsSpeedAngR = new FuzzySet("saR", new TrapezoidalFunction(0.5f, 1f, 1.5f));

            /*FuzzySet fsRNear = new FuzzySet("mf1", new TrapezoidalFunction(0, 0.5f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsRFar = new FuzzySet("mf2", new TrapezoidalFunction(0f, 0.5f, 1f));
            FuzzySet fsRB = new FuzzySet("mf3", new TrapezoidalFunction(0.5f, 2f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsLNear = new FuzzySet("mf1", new TrapezoidalFunction(0, 0.5f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsLFar = new FuzzySet("mf2", new TrapezoidalFunction(0f, 0.5f, 1f));
            FuzzySet fsLB = new FuzzySet("mf3", new TrapezoidalFunction(0.5f, 2f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsN = new FuzzySet("Negative", new TrapezoidalFunction(-2f, -2.5f, -3f));
            FuzzySet fsZero = new FuzzySet("Zero", new TrapezoidalFunction(-0.1f, 0, 0.1f));
            FuzzySet fsP = new FuzzySet("Positive", new TrapezoidalFunction(2f, 2.5f, 3.5f, 4f));*/

           /* FuzzySet fsS1 = new FuzzySet("nh", new TrapezoidalFunction(-4f, -3.5f, -1.5f));
            FuzzySet fsS2 = new FuzzySet("nn", new TrapezoidalFunction(-3f, -1.5f, 0f));
            FuzzySet fsS3 = new FuzzySet("pp", new TrapezoidalFunction(-1f, 0f, 1f));
            FuzzySet fsS4 = new FuzzySet("hp", new TrapezoidalFunction(0f, 1.5f, 3f));
            FuzzySet fsS5 = new FuzzySet("vhp", new TrapezoidalFunction(1.5f, 3.5f, 4f));*/

            FuzzySet fsS1 = new FuzzySet("nh", new TrapezoidalFunction(-4f, -3.5f, -1.5f));
            FuzzySet fsS2 = new FuzzySet("nn", new TrapezoidalFunction(-3f, -1.5f, -0.2f));
            FuzzySet fsS3 = new FuzzySet("pp", new TrapezoidalFunction(-1f, 0f, 1f));
            FuzzySet fsS4 = new FuzzySet("hp", new TrapezoidalFunction(0.2f, 1.5f, 3f));
            FuzzySet fsS5 = new FuzzySet("vhp", new TrapezoidalFunction(1.5f, 3.5f, 4f));

            LinguisticVariable lvFront = new LinguisticVariable("FrontalDistance", -0.1f, 100);

            lvFront.AddLabel(fsNear);
            lvFront.AddLabel(fsFar);
            lvFront.AddLabel(fsB);

            LinguisticVariable lvRignt = new LinguisticVariable("RightDistance", -0.1f, 100);

            lvRignt.AddLabel(fsNear);
            lvRignt.AddLabel(fsFar);
            lvRignt.AddLabel(fsB);


            LinguisticVariable lvLeft = new LinguisticVariable("LeftDistance", -0.1f, 100);

            lvLeft.AddLabel(fsNear);
            lvLeft.AddLabel(fsFar);
            lvLeft.AddLabel(fsB);

            LinguisticVariable lvSpeedAngle = new LinguisticVariable("SpeedAngle", -1.5f, 1.5f);

            lvSpeedAngle.AddLabel(fsSpeedAngL);
            lvSpeedAngle.AddLabel(fsSpeedAng);
            lvSpeedAngle.AddLabel(fsSpeedAngR);

            LinguisticVariable lvUngl = new LinguisticVariable("Ungle", -180f, 180f);

            lvUngl.AddLabel(fsUngL);
            lvUngl.AddLabel(fsUng);
            lvUngl.AddLabel(fsUngR);

            /*LinguisticVariable lvSpeedR = new LinguisticVariable("SpeedR", -4, 4);

            lvSpeedR.AddLabel(fsS1);
            lvSpeedR.AddLabel(fsS2);
            lvSpeedR.AddLabel(fsS3);
            lvSpeedR.AddLabel(fsS4);
            lvSpeedR.AddLabel(fsS5);*/

            LinguisticVariable lvSpeedL = new LinguisticVariable("SpeedL", -4, 4);

            lvSpeedL.AddLabel(fsS1);
            lvSpeedL.AddLabel(fsS2);
            lvSpeedL.AddLabel(fsS3);
            lvSpeedL.AddLabel(fsS4);
            lvSpeedL.AddLabel(fsS5);



            //LinguisticVariable lvAlpha = new LinguisticVariable("Alpha", -180f, 180f);
            //lvAlpha.AddLabel(fsL);
            //lvAlpha.AddLabel(fsC);
            //lvAlpha.AddLabel(fsR);

            //LinguisticVariable lvAngle = new LinguisticVariable("Angle", -10, 10);


            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvFront);
            fuzzyDB.AddVariable(lvRignt);
            fuzzyDB.AddVariable(lvLeft);
            fuzzyDB.AddVariable(lvSpeedAngle);
            fuzzyDB.AddVariable(lvUngl);
            //fuzzyDB.AddVariable(lvSpeedR);
            fuzzyDB.AddVariable(lvSpeedL);

            IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));
            //IS.NewRule("Rule", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh");
            /*IS.NewRule("Rule 1", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS nh");
            IS.NewRule("Rule 2", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS n AND SpeedR IS nh");
            IS.NewRule("Rule 3", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS n AND SpeedR IS nh");
            IS.NewRule("Rule 4", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS nh");
            IS.NewRule("Rule 5", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS n AND SpeedR IS nh");
            IS.NewRule("Rule 6", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS n AND SpeedR IS nh");
            IS.NewRule("Rule 7", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS nh");
            IS.NewRule("Rule 8", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS n AND SpeedR IS nh");
            IS.NewRule("Rule 9", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS n AND SpeedR IS nh");

            IS.NewRule("Rule 10", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS n");
            IS.NewRule("Rule 11", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nh AND SpeedR IS nh");
            IS.NewRule("Rule 12", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS vhp AND SpeedR IS p");
            IS.NewRule("Rule 13", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS p AND SpeedR IS vhp");
            IS.NewRule("Rule 14", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS vhp AND SpeedR IS p");
            IS.NewRule("Rule 15", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp AND SpeedR IS p");
            IS.NewRule("Rule 16", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS n");
            IS.NewRule("Rule 17", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS vhp AND SpeedR IS p");
            IS.NewRule("Rule 18", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS vhp AND SpeedR IS p");
            
            IS.NewRule("Rule 19", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS n");
            IS.NewRule("Rule 20", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS p AND SpeedR IS vhp");
            IS.NewRule("Rule 21", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS nh AND SpeedR IS nh");
            IS.NewRule("Rule 22", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS n");
            IS.NewRule("Rule 23", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS p AND SpeedR IS vhp");
            IS.NewRule("Rule 24", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp AND SpeedR IS p");
            IS.NewRule("Rule 25", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh AND SpeedR IS n");
            IS.NewRule("Rule 26", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS p AND SpeedR IS vhp");
            IS.NewRule("Rule 27", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS hp AND SpeedR IS hp");
            */
            /*IS.NewRule("Rule 1", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh");
            IS.NewRule("Rule 2", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nn");
            IS.NewRule("Rule 3", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS nn ");
            IS.NewRule("Rule 4", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 5", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS nn ");
            IS.NewRule("Rule 6", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS nn ");
            IS.NewRule("Rule 7", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 8", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS nn ");
            IS.NewRule("Rule 9", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS nn ");

            IS.NewRule("Rule 10", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh");
            IS.NewRule("Rule 11", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 12", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 13", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS pp");
            IS.NewRule("Rule 14", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 15", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 16", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 17", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 18", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS vhp ");

            IS.NewRule("Rule 19", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 20", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS pp ");
            IS.NewRule("Rule 21", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS nh ");
            IS.NewRule("Rule 22", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 23", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS pp ");
            IS.NewRule("Rule 24", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 25", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 26", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS pp ");
            IS.NewRule("Rule 27", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS hp ");






            IS.NewRule("Rule 01", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS nh");
            IS.NewRule("Rule 02", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS nh");
            IS.NewRule("Rule 03", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS nh");
            IS.NewRule("Rule 04", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS nh");
            IS.NewRule("Rule 05", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS nh");
            IS.NewRule("Rule 06", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS nh");
            IS.NewRule("Rule 07", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS nh");
            IS.NewRule("Rule 08", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS nh");
            IS.NewRule("Rule 09", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS nh");

            IS.NewRule("Rule 010", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 011", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS nh");
            IS.NewRule("Rule 012", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS pp");
            IS.NewRule("Rule 013", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 014", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS pp");
            IS.NewRule("Rule 015", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS pp");
            IS.NewRule("Rule 016", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 017", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS pp");
            IS.NewRule("Rule 018", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS pp");

            IS.NewRule("Rule 019", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 020", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 021", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS nh");
            IS.NewRule("Rule 022", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 023", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 024", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS pp");
            IS.NewRule("Rule 025", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 026", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 027", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS hp");
            */



           /* IS.NewRule("Rule 1", "IF (FrontalDistance IS mf3 and Ungle IS u3) OR (FrontalDistance IS mf3 AND RightDistance IS mf3  AND Ungle IS u3) OR (FrontalDistance IS mf1"+
            " AND (Ungle IS u3 OR Ungle IS u2)) OR (FrontalDistance IS mf1 AND RightDistance IS mf1 AND LeftDistance IS mf1)"+
            " Then SpeedAngle IS saR");
            IS.NewRule("Rule 2", "IF (FrontalDistance IS mf3 and Ungle IS u1) OR (FrontalDistance IS mf3 AND LeftDistance IS mf3  AND Ungle IS u1) OR (FrontalDistance IS mf1" +
            " AND (Ungle IS u1)" +
            " Then SpeedAngle IS saL");
            IS.NewRule("Rule 3", "IF FrontalDistance Is mf3 AND (RightDistance IS mf1 OR LeftDistance IS mf1"+
                " OR (RightDistance IS mf1 AND LeftDistance IS mf1))"+
                " THEN SpeedAngle IS sa");*/
            IS.NewRule("Rule 4", "IF FrontalDistance IS mf3 THEN SpeedL IS vhp");
            IS.NewRule("Rule 5", "IF FrontalDistance IS mf2 THEN SpeedL IS hp");
            IS.NewRule("Rule 6", "IF FrontalDistance IS mf1 THEN SpeedL IS pp");

            
            /*IS.NewRule("Rule 1", "IF FrontalDistance IS mf1  THEN SpeedL IS pp");
            IS.NewRule("Rule 01", "IF FrontalDistance IS mf3  AND Ungle IS u2  THEN SpeedAngle IS sa");
            IS.NewRule("Rule 2", "IF FrontalDistance IS mf1 AND LeftDistance IS mf3 AND   RightDistance IS mf3 AND Ungle IS u1 THEN SpeedAngle IS saL");
            IS.NewRule("Rule 3", "IF FrontalDistance IS mf1 AND LeftDistance IS mf3 AND   RightDistance IS mf3 AND Ungle IS u2 THEN SpeedAngle IS saR");
            IS.NewRule("Rule 4", "IF FrontalDistance IS mf1 AND LeftDistance IS mf3 AND   RightDistance IS mf3 AND Ungle IS u3 THEN SpeedAngle IS saR");
            IS.NewRule("Rule 5", "IF FrontalDistance IS mf1 AND LeftDistance IS mf1 AND   RightDistance IS mf3   THEN SpeedAngle IS saR");
            IS.NewRule("Rule 6", "IF FrontalDistance IS mf1 AND LeftDistance IS mf3 AND   RightDistance IS mf1   THEN SpeedAngle IS saL");
            IS.NewRule("Rule 7", "IF FrontalDistance IS mf3 THEN SpeedL IS vhp");
            //IS.NewRule("Rule 07", "IF FrontalDistance IS mf3  AND Ungle IS u2 THEN SpeedAngle IS sa");
            IS.NewRule("Rule 8", "IF FrontalDistance IS mf2 THEN SpeedL IS hp");
            IS.NewRule("Rule 9", "IF FrontalDistance IS mf3 AND LeftDistance IS mf3 AND Ungle IS u1 THEN SpeedAngle IS saL");
            IS.NewRule("Rule 10", "IF FrontalDistance IS mf3  AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedAngle IS saR");
            IS.NewRule("Rule 11", "IF FrontalDistance IS mf3  AND LeftDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedAngle IS saL");
            IS.NewRule("Rule 12", "IF FrontalDistance IS mf3  AND LeftDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedAngle IS saR");
           */
            /////////////////////////////////////////////////////////////////////////////////////////////////
            /*IS.NewRule("Rule 1", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh");
            IS.NewRule("Rule 2", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nh");
            IS.NewRule("Rule 3", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 4", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 5", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 6", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 7", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 8", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS vhp ");
            //
            IS.NewRule("Rule 9", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 9a", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 9b", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedL IS vhp ");

            IS.NewRule("Rule 10", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh");
            IS.NewRule("Rule 11", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 12", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 13", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh");
            IS.NewRule("Rule 14", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 15", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 16", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedL IS hp ");
            IS.NewRule("Rule 17", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedL IS vhp ");
            IS.NewRule("Rule 18", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN SpeedL IS vhp ");

            IS.NewRule("Rule 19", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 20", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            //
            IS.NewRule("Rule 21", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 21a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 21b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedL IS vhp ");
            //
            IS.NewRule("Rule 22", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 23", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedL IS nh ");
            //
            IS.NewRule("Rule 24", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedL IS nh ");
            IS.NewRule("Rule 24a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedL IS nh ");
            IS.NewRule("Rule 24b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedL IS vhp ");
            //
            IS.NewRule("Rule 25", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u1  THEN SpeedL IS nh ");
            IS.NewRule("Rule 25a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u2  THEN SpeedL IS vhp ");
            IS.NewRule("Rule 25b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u3  THEN SpeedL IS vhp ");
            //
            IS.NewRule("Rule 26", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u1  THEN SpeedL IS nh ");
            IS.NewRule("Rule 26a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u2  THEN SpeedL IS vhp ");
            IS.NewRule("Rule 26b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u3  THEN SpeedL IS vhp ");
            //
            IS.NewRule("Rule 27", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1  THEN SpeedL IS nh ");
            IS.NewRule("Rule 27a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u2  THEN SpeedL IS vhp ");
            IS.NewRule("Rule 27b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3  THEN SpeedL IS vhp ");


            ///////////////////////////////////////////////////////////////////////




            IS.NewRule("Rule 01", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedR IS nh");
            IS.NewRule("Rule 02", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedR IS nh");
            IS.NewRule("Rule 03", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedR IS nh ");
            IS.NewRule("Rule 04", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedR IS nh ");
            IS.NewRule("Rule 05", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedR IS nh ");
            IS.NewRule("Rule 06", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedR IS nh ");
            IS.NewRule("Rule 07", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 08", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedR IS hp ");
            //
            IS.NewRule("Rule 09", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 09a", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 09b", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedR IS nh ");

            IS.NewRule("Rule 010", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedR IS nh");
            IS.NewRule("Rule 011", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedR IS nh ");
            IS.NewRule("Rule 012", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN SpeedR IS nh ");
            IS.NewRule("Rule 013", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedR IS nh");
            IS.NewRule("Rule 014", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedR IS nh ");
            IS.NewRule("Rule 015", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN SpeedR IS nh ");
            IS.NewRule("Rule 016", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 017", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN SpeedR IS vhp ");
            //
            IS.NewRule("Rule 018", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 018a", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 018b", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedR IS nh ");
            //

            IS.NewRule("Rule 019", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 020", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN SpeedR IS vhp ");
            //
            IS.NewRule("Rule 021", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 021a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 021b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedR IS nh ");
            //
            IS.NewRule("Rule 022", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 023", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN SpeedR IS vhp ");
            //
            IS.NewRule("Rule 024", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u1 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 024a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u2 THEN SpeedR IS vhp ");
            IS.NewRule("Rule 024b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 AND Ungle IS u3 THEN SpeedR IS nh ");
            //
            IS.NewRule("Rule 025", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u1  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 025a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u2  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 025b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 AND Ungle IS u3  THEN SpeedR IS vhp ");
            //
            IS.NewRule("Rule 026", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u1  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 026a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u2  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 026b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 AND Ungle IS u3  THEN SpeedR IS vhp ");
            //
            IS.NewRule("Rule 027", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u1  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 027a", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u2  THEN SpeedR IS vhp ");
            IS.NewRule("Rule 027b", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 AND Ungle IS u3  THEN SpeedR IS nh ");
            */
            /*
            IS.NewRule("Rule 01", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS nh");
            IS.NewRule("Rule 02", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS pp");
            IS.NewRule("Rule 03", "IF LeftDistance IS mf1 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS pp");
            IS.NewRule("Rule 04", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 05", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS nn");
            IS.NewRule("Rule 06", "IF LeftDistance IS mf1 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS hp");
            IS.NewRule("Rule 07", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS hp");
            IS.NewRule("Rule 08", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS hp");
            IS.NewRule("Rule 09", "IF LeftDistance IS mf1 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS vhp");

            IS.NewRule("Rule 010", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS nh");
            IS.NewRule("Rule 011", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS nn");
            IS.NewRule("Rule 012", "IF LeftDistance IS mf2 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS nn");
            IS.NewRule("Rule 013", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS nn");
            IS.NewRule("Rule 014", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS nn");
            IS.NewRule("Rule 015", "IF LeftDistance IS mf2 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS nn");
            IS.NewRule("Rule 016", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 017", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS hp");
            IS.NewRule("Rule 018", "IF LeftDistance IS mf2 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS hp");

            IS.NewRule("Rule 019", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf1 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 020", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf2 THEN  SpeedR IS hp");
            IS.NewRule("Rule 021", "IF LeftDistance IS mf3 AND FrontalDistance IS mf1 AND RightDistance IS mf3 THEN  SpeedR IS nn");
            IS.NewRule("Rule 022", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf1 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 023", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf2 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 024", "IF LeftDistance IS mf3 AND FrontalDistance IS mf2 AND RightDistance IS mf3 THEN  SpeedR IS nn");
            IS.NewRule("Rule 025", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf1 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 026", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf2 THEN  SpeedR IS vhp");
            IS.NewRule("Rule 027", "IF LeftDistance IS mf3 AND FrontalDistance IS mf3 AND RightDistance IS mf3 THEN  SpeedR IS vhp");*/

        }

        public void Calculate(float dist, float distL, float distR, float ang)
        {
            IS.SetInput("FrontalDistance", dist);
            IS.SetInput("LeftDistance", distL);
            IS.SetInput("RightDistance", distR);
            IS.SetInput("Ungle", ang);
            /*try
            {
                return IS.Evaluate("Speed");
            }
            catch (Exception)
            {
                return 0;
            }*/

        }

        public float CalculateRS()
        {
            try
            {
                return IS.Evaluate("SpeedR");
            }
            catch (Exception)
            {
                return -1000;
            }
        }

        public float CalculateLS()
        {
            try
            {
                //return IS.Evaluate("SpeedL");
                var c = IS.Evaluate("SpeedL");
                if ((c > -0.1f) && (c < 0.1f))
                    c = 0;
                return c;
            }
            catch (Exception)
            {
                return -1000;
            }
        }

        public float CalculateA()
        {
            try
            {
                var c = IS.Evaluate("SpeedAngle");
                if ((c > -0.1f) && (c < 0.1f))
                    c = 0;
                return c;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1000;
            }
        }
    }

    /*class FuzzyLogicDriveToPoint
    {
        InferenceSystem IS;

        public FuzzyLogicDriveToPoint()
        {
            InitializeFuzzy();
        }

        public void InitializeFuzzy()
        {
            //FuzzySet fsDNN = new FuzzySet("mf1", new TrapezoidalFunction(-180f, -140f, TrapezoidalFunction.EdgeType.Left));
            //FuzzySet fsDN = new FuzzySet("mf2", new TrapezoidalFunction(-180f, -120f, -60f));
            //FuzzySet fsDLN = new FuzzySet("mf3", new TrapezoidalFunction(-140f, -55f, 0f));
            //FuzzySet fsD = new FuzzySet("mf4", new TrapezoidalFunction(-60f, 0f, 60f));
            //FuzzySet fsDLF = new FuzzySet("mf5", new TrapezoidalFunction(0f, 55f, 140f));
            //FuzzySet fsDF = new FuzzySet("mf6", new TrapezoidalFunction(60f, 120f, 180f));
            //FuzzySet fsDFF = new FuzzySet("mf7", new TrapezoidalFunction(140f, 180f, TrapezoidalFunction.EdgeType.Right));

            FuzzySet fsDNN = new FuzzySet("mf1", new TrapezoidalFunction(0, 1f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsDN = new FuzzySet("mf2", new TrapezoidalFunction(0f, 1f, 2f));
            FuzzySet fsDLN = new FuzzySet("mf3", new TrapezoidalFunction(1f, 2f, 3f));
            FuzzySet fsD = new FuzzySet("mf4", new TrapezoidalFunction(2f, 3f, 4f));
            FuzzySet fsDLF = new FuzzySet("mf5", new TrapezoidalFunction(3f, 4f, 5f));
            FuzzySet fsDF = new FuzzySet("mf6", new TrapezoidalFunction(4f, 5f, 6f));
            FuzzySet fsDFF = new FuzzySet("mf7", new TrapezoidalFunction(5f, 6f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsALF = new FuzzySet("mf1", new TrapezoidalFunction(-180f, -140f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsAL = new FuzzySet("mf2", new TrapezoidalFunction(-180f, -120f, -60f));
            FuzzySet fsALN = new FuzzySet("mf3", new TrapezoidalFunction(-140f, -55f, 0f));
            FuzzySet fsA = new FuzzySet("mf4", new TrapezoidalFunction(-60f, 0f, 60f));
            FuzzySet fsARN = new FuzzySet("mf5", new TrapezoidalFunction(0f, 55f, 140f));
            FuzzySet fsAR = new FuzzySet("mf6", new TrapezoidalFunction(60f, 120f, 180f));
            FuzzySet fsARF = new FuzzySet("mf7", new TrapezoidalFunction(140f, 180f, TrapezoidalFunction.EdgeType.Left));

            FuzzySet fsN = new FuzzySet("Negative", new TrapezoidalFunction(-2f, -2.5f, -3f));
            FuzzySet fsZero = new FuzzySet("Zero", new TrapezoidalFunction(-0.1f, 0, 0.1f));
            FuzzySet fsP = new FuzzySet("Positive", new TrapezoidalFunction(2f, 2.5f, 3.5f, 4f));

            FuzzySet fsL = new FuzzySet("Left", new TrapezoidalFunction(-10f, -30f, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsC = new FuzzySet("Centr", new TrapezoidalFunction(-15f, 0, 15f));
            FuzzySet fsR = new FuzzySet("Right", new TrapezoidalFunction(10f, 30f, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvDist = new LinguisticVariable("Distance", -1f, 100);

            lvDist.AddLabel(fsNear);
            lvDist.AddLabel(fsFar);
            lvDist.AddLabel(fsB);
            lvDist.AddLabel(fsNear);
            lvDist.AddLabel(fsFar);
            lvDist.AddLabel(fsB);
            lvDist.AddLabel(fsB);

            LinguisticVariable lvAlpha = new LinguisticVariable("Alpha", -180f, 180f);
            lvAlpha.AddLabel(fsL);
            lvAlpha.AddLabel(fsC);
            lvAlpha.AddLabel(fsR);

            LinguisticVariable lvSpeedR = new LinguisticVariable("SpeedRB", -10, 10);

            lvSpeed.AddLabel(fsZero);
            lvSpeed.AddLabel(fsP);
            lvSpeed.AddLabel(fsN);

            LinguisticVariable lvSpeedL = new LinguisticVariable("SpeeLB", -10, 10);


            //LinguisticVariable lvAlpha = new LinguisticVariable("Alpha", -180f, 180f);
            //lvAlpha.AddLabel(fsL);
            //lvAlpha.AddLabel(fsC);
            //lvAlpha.AddLabel(fsR);

            //LinguisticVariable lvAngle = new LinguisticVariable("Angle", -10, 10);


            //Database fuzzyDB = new Database();
            //fuzzyDB.AddVariable(lvFront);
            //fuzzyDB.AddVariable(lvSpeed);

            IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

            IS.NewRule("Rule 1", "IF FrontalDistance IS Near THEN Speed IS Zero");
            IS.NewRule("Rule 2", "IF FrontalDistance IS Far THEN Speed IS Positive");
            IS.NewRule("Rule 3", "IF FrontalDistance IS Late THEN Speed IS Negative");
        }

        public float GetSpeed(float dist)
        {
            IS.SetInput("FrontalDistance", dist);
            try
            {
                return IS.Evaluate("Speed");
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }*/
}
