using UnityEngine;

namespace Assets.Scripts.UI
{
	public class ScaleToFitScreenAnyCam : MonoBehaviour
	{
		[SerializeField] private BoxCollider _sizeCollider;
		[SerializeField] private Transform _target;

		[SerializeField] private bool _keepAspectRatio = false;
		[SerializeField] private bool _expandWithAspect = true;

		private const float CameraDistance = 200;

		private Camera _camera;
		private float _addBorder = .2f;

		private ResizeCalculations _lastResizeCalculations;

		private float _colliderWidthInitial;
		private float _colliderHeightInitial;

        private void Start()
        {
			Init();
        }

		private void OnDestroy()
		{
			if (Game.Instance)
				Game.Instance.ScreenResized -= OnScreenResize;
		}

		public void Init()
		{
			_camera = Camera.main;

			SetupInitialSize();
			Game.Instance.ScreenResized += OnScreenResize;
			OnScreenResize();
		}

		private void SetupInitialSize()
		{
			_target.localScale = Vector3.one;
			_colliderWidthInitial = _sizeCollider.bounds.size.x * transform.localScale.x;
			_colliderHeightInitial = _sizeCollider.bounds.size.y * transform.localScale.y;
		}

		private bool NeedsResize(out ResizeCalculations c)
		{
			if (!_camera)
			{
				_camera = Camera.main;
				if (!_camera)
				{
					c = new ResizeCalculations();
					return false;
				}
			}

			var midPoint = _camera.ViewportToWorldPoint(new Vector3(0.5f, .5f, CameraDistance));

			var width = _colliderWidthInitial;
			var height = _colliderHeightInitial;

			var camZero = _camera.ViewportToWorldPoint(new Vector3(0, 0, CameraDistance));
			var camOne = _camera.ViewportToWorldPoint(new Vector3(1, 1, CameraDistance));

			var worldScreenHeight = (camOne.y - camZero.y) + _addBorder;
			var worldScreenWidth = (camOne.x - camZero.x) + _addBorder;

			if (_lastResizeCalculations.midPos != midPoint ||
				_lastResizeCalculations.widthCollider != width || _lastResizeCalculations.heightCollider != height ||
				_lastResizeCalculations.widthWorldNeed != worldScreenWidth ||
				_lastResizeCalculations.heightWorldNeed != worldScreenHeight)
			{
				c = new ResizeCalculations()
				{
					midPos = midPoint,

					widthWorldNeed = worldScreenWidth,
					heightWorldNeed = worldScreenHeight,

					widthCollider = width,
					heightCollider = height
				};
				return true;
			}

			c = new ResizeCalculations();
			return false;
		}

		private void ResizeForTarget(ResizeCalculations calculations)
		{
			_lastResizeCalculations = calculations;

			_target.position = calculations.midPos;

			var width = calculations.widthCollider;
			var height = calculations.heightCollider;

			var worldScreenHeight = calculations.heightWorldNeed;
			var worldScreenWidth = calculations.widthWorldNeed;

			Vector3 scaleFactor;

			if (!_keepAspectRatio)
			{
				var scaleY = height != 0 ? worldScreenHeight / height : 0;
				var scaleX = width != 0 ? worldScreenWidth / width : 0;
				scaleFactor = new Vector3(scaleX, scaleY, 1);
			}
			else
			{
				var aspectCam = worldScreenWidth / worldScreenHeight;
				var aspectTarget = width / height;

				if (_expandWithAspect && aspectCam >= aspectTarget || !_expandWithAspect && aspectCam < aspectTarget)
				{
					// скейл по ширине
					var s = worldScreenWidth / width;
					scaleFactor = new Vector3(s, s, 1);
				}
				else
				{
					// скейл по высоте
					var s = worldScreenHeight / height;
					scaleFactor = new Vector3(s, s, 1);
				}
			}

			_target.localScale = scaleFactor;
		}

		private void OnScreenResize()
		{
			if (NeedsResize(out var calculations))
				ResizeForTarget(calculations);
		}

		private struct ResizeCalculations
		{
			public Vector3 midPos;
			public float widthCollider;
			public float heightCollider;
			public float widthWorldNeed;
			public float heightWorldNeed;
		}
	}
}