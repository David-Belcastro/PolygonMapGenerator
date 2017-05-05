using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaMapGenerator3D.Services
{
    public class CameraService
    {
        private BGame _game;
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        private MouseState newMouseState, oldMouseState;
        private int totalCamPitch;
        private int totalCamYaw;
        private Matrix cameraRotation;
        public Vector3 CameraPosition { get; set; }
        public Vector3 LookPosition { get; set; }
        public float Zoom { get; set; }
        

        public CameraService(BGame game)
        {
            _game = game;
            Zoom = 1700;
            CameraPosition = new Vector3(1600, 1400, 1200);
            LookPosition = new Vector3(1600, 0, 1600);

            ViewMatrix = Matrix.CreateLookAt(CameraPosition, LookPosition, new Vector3(0, 1, 0));
            //ProjectionMatrix = Matrix.CreateOrthographicOffCenter(-400, 400, -240, 240, 0.2f, 15000.0f);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _game.GraphicsDevice.Viewport.AspectRatio, 0.2f, 15000.0f);
            

            Matrix worldMatrix = Matrix.Identity;
            //_game.ShaderEffect.CurrentTechnique = _game.ShaderEffect.Techniques["Textured"];
            //_game.ShaderEffect.Parameters["xWorld"].SetValue(worldMatrix);
            //_game.ShaderEffect.Parameters["xView"].SetValue(ViewMatrix);
            //_game.ShaderEffect.Parameters["xProjection"].SetValue(ProjectionMatrix);

            _game.ShaderEffect.World = worldMatrix;
            _game.ShaderEffect.View = ViewMatrix;
            _game.ShaderEffect.Projection = ProjectionMatrix;

            //Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            //lightDirection.Normalize();
            //_game.ShaderEffect.Parameters["xLightDirection"].SetValue(lightDirection);
            //_game.ShaderEffect.Parameters["xAmbient"].SetValue(0.5f);
            //_game.ShaderEffect.Parameters["xEnableLighting"].SetValue(true);
            //_game.ShaderEffect.Parameters["xShowNormals"].SetValue(true);  

            //_game.ShaderEffect.Alpha = 1f;
            //_game.ShaderEffect.DiffuseColor = new Vector3(1f, 1f, 1f);
            //_game.ShaderEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            ////_game.ShaderEffect.SpecularColor = new Vector3(1.00f, 1.00f, 1.00f);
            //_game.ShaderEffect.SpecularPower = 0.2f;

            var lightPos = new Vector3(400, 400, 400);
            var lightPower = 1.0f;
            var ambientPower = 0.4f;

            //_game.ShaderEffect.Parameters["xWorld"].SetValue(worldMatrix);
            //_game.ShaderEffect.Parameters["xLightPos"].SetValue(lightPos);
            //_game.ShaderEffect.Parameters["xLightPower"].SetValue(lightPower);
            //_game.ShaderEffect.Parameters["xAmbient"].SetValue(ambientPower);
            //_game.ShaderEffect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * ViewMatrix * ProjectionMatrix);

            _game.ShaderEffect.DirectionalLight0.Direction = new Vector3(-0.7f, -0.7f, -0.7f);
            _game.ShaderEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 0.9f, 0.6f);
            _game.ShaderEffect.DirectionalLight0.Enabled = true;

            _game.ShaderEffect.DirectionalLight1.Direction = new Vector3(0.4f, -0.4f, 0.4f);
            _game.ShaderEffect.DirectionalLight1.DiffuseColor = new Vector3(0.6f, 0.9f, 1f);
            _game.ShaderEffect.DirectionalLight1.Enabled = true;

            _game.ShaderEffect.DirectionalLight2.Direction = new Vector3(-1.0f, 1.0f, 1.0f);
            _game.ShaderEffect.DirectionalLight2.DiffuseColor = new Vector3(1f, 1f, 1f);
            _game.ShaderEffect.DirectionalLight2.Enabled = false;

            _game.ShaderEffect.LightingEnabled = true;
            _game.ShaderEffect.PreferPerPixelLighting = true;

            //_game.PerPixelEffect.Parameters["world"].SetValue(worldMatrix);
            //_game.PerPixelEffect.Parameters["view"].SetValue(ViewMatrix);
            //_game.PerPixelEffect.Parameters["projection"].SetValue(ProjectionMatrix);
            //_game.PerPixelEffect.Parameters["cameraPosition"].SetValue(CameraPosition);
            //_game.PerPixelEffect.Parameters["specularPower"].SetValue(1);
            //_game.PerPixelEffect.Parameters["specularIntensity"].SetValue(1);

            //_game.PerPixelEffect.Parameters["ambientLightColor"].SetValue(
            //        Color.DarkSlateGray.ToVector4());
            //_game.PerPixelEffect.Parameters["diffuseLightColor"].SetValue(
            //    Color.CornflowerBlue.ToVector4());
            //_game.PerPixelEffect.Parameters["specularLightColor"].SetValue(
            //    Color.White.ToVector4());

            //_game.PerPixelEffect.Parameters["lightPosition"].SetValue(
            //        new Vector3(400f, 400f, 400f));
            
        }

        public void Update()
        {
            oldMouseState = newMouseState;
            newMouseState = Mouse.GetState();

            var a = newMouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;
            Zoom += a;
            Vector3 cameraOffset = new Vector3(0, 0, Zoom); 


            if (newMouseState.LeftButton == ButtonState.Pressed)
            {
                //set mouse invissible (not really needed) 
                _game.IsMouseVisible = false;
                //Simply set the total camera pitch and yaw to the difference in mouseposition, use -= to invert 
                
                totalCamPitch += (oldMouseState.Y - newMouseState.Y);
                totalCamYaw += (oldMouseState.X - newMouseState.X);

                //Set maximum pith to 89 degree's, read Google gimbal lock if you want to know why. 
                if (totalCamPitch >= 89)
                {
                    totalCamPitch = 89;
                }
                if (totalCamPitch <= -89)
                {
                    totalCamPitch = -89;
                }
            }
            else
            {
                // put mouse back on if mouse button is released. 
                _game.IsMouseVisible = true;
            }

            //sets the camera rotation x and y equal to the total yaw and pitch 
            cameraRotation = Matrix.CreateRotationX(MathHelper.ToRadians(totalCamPitch)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(totalCamYaw));

            // sets the camera position. 
            Vector3 cameraRotatedPosition = Vector3.Transform(cameraOffset, cameraRotation);

            // finally sets the camera position behind the player 
            CameraPosition = cameraRotatedPosition + LookPosition;

            ViewMatrix = Matrix.CreateLookAt(CameraPosition, LookPosition, new Vector3(0, 1, 0));
            //_game.ShaderEffect.Parameters["xView"].SetValue(ViewMatrix);

            //_game.ShaderEffect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * ViewMatrix * ProjectionMatrix);
            _game.ShaderEffect.View = ViewMatrix;
            //_game.PerPixelEffect.Parameters["view"].SetValue(ViewMatrix);
            //_game.PerPixelEffect.Parameters["cameraPosition"].SetValue(CameraPosition);
        }
    }
}
