//*********************************************************************************
//      DefaultValueDefMapAction.cs
//*********************************************************************************
// File Name: PortDefaultValueDefMapAction.cs
// Description: Port Scenario 
//
//(c) Copyright 2013, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    /// <summary>
    /// Class PortDefaultValueDefMapAction.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.ValueDefMapAction.ValueDefMapActionBase" />
    public class PortDefaultValueDefMapAction : ValueDefMapActionBase
    {
        /// <summary>
        /// The portstatus log
        /// </summary>
        protected Logger portstatusLog;
        /// <summary>
        /// The port
        /// </summary>
        protected APORT port = null;
        //        protected Equipment eqpt = null;
        /// <summary>
        /// The node
        /// </summary>
        protected ANODE node = null;

        //A0.01        protected LDCMValueHandler ldcmValueHandler = null;
        //LDCM回覆後，RecipeID順序
        /// <summary>
        /// The recipe identifier nodes
        /// </summary>
        protected String[] recipeIDNodes = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortDefaultValueDefMapAction"/> class.
        /// </summary>
        public PortDefaultValueDefMapAction()
            : base()
        {

        }

        /// <summary>
        /// Gets the identity key.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string getIdentityKey()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="baseEQ">The base eq.</param>
        public override void setContext(BaseEQObject baseEQ)
        {
            this.port = baseEQ as APORT;
            doTestEQ();
        }

        /// <summary>
        /// Uns the register event.
        /// </summary>
        public override void unRegisterEvent()
        {
            // not implement
        }

        /// <summary>
        /// Does the share memory initialize.
        /// </summary>
        /// <param name="runLevel">The run level.</param>
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:

                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }




        /// <summary>
        /// Does the initialize.
        /// </summary>
        public override void doInit()
        {
            try
            {
                scApp.getBCFApplication();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }



    }
}
