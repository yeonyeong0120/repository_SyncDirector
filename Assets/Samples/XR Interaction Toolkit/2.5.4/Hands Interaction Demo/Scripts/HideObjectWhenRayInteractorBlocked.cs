using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit; // XRBaseInteractor를 사용하려면 필요할 수 있지만, 여기서는 XRRayInteractor만 사용합니다.

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Hides the specified GameObject when the associated XRRayInteractor is **not** the primary interaction source,
    /// or when it is disabled.
    ///
    /// NOTE: Replaces the obsolete IsBlockedByInteractionWithinGroup() check with
    /// a simple check for whether the Interactor is active and enabled.
    /// </summary>
    public class HideObjectWhenRayInteractorBlocked : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRRayInteractor that this component monitors for blockages.")]
        XRRayInteractor m_Interactor;

        [SerializeField]
        [Tooltip("The GameObject to hide when the XRRayInteractor is blocked (i.e., not active/enabled).")]
        GameObject m_ObjectToHide;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            // 필수 컴포넌트가 할당되지 않았다면 스크립트 비활성화
            if (m_Interactor == null || m_ObjectToHide == null)
                enabled = false;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            // 원래 로직: m_ObjectToHide.SetActive(m_Interactor.isActiveAndEnabled && !m_Interactor.IsBlockedByInteractionWithinGroup());

            // 수정된 로직: Ray Interactor가 활성화(Active)되어 있고,
            // 현재 어떤 상호작용 가능한 오브젝트(Interactable)도 선택하지 않은 경우에만 오브젝트를 표시합니다.

            // XRI 2.5.4 버전에서는 Ray Interactor의 '활성화 상태'가 Ray가 사용 가능한지 여부를 가장 잘 나타냅니다.
            // 여기서는 단순히 Interactor가 활성화되어 있고 컴포넌트 자체가 활성화되어 있는지 확인합니다.

            // Interactor의 isActiveAndEnabled 속성은 Interactor가 현재 입력 시스템에 의해
            // 실제로 사용 가능한 상태인지를 간접적으로 나타냅니다.

            // 만약 Ray가 **다른 Interactor에 의해 차단된 경우**를 명확히 구현하려면,
            // 'XR Interaction Group'의 `CanSelect()` 또는 `CanHover()` 메서드를 활용하여
            // Interactor의 상태를 확인하는 복잡한 로직이 필요할 수 있습니다.

            // 간단하게는 Ray Interactor가 현재 Active한 상태인지 확인합니다.
            // !m_Interactor.hasSelection : Ray가 현재 오브젝트를 잡고 있지 않은 경우 (선택/집기가 아님)
            // !m_Interactor.hasHover : Ray가 현재 오브젝트 위에 있지 않은 경우 (Hover가 아님)

            // 가장 안전한 대안: Interactor가 활성화되어 있고 어떤 오브젝트도 선택하고 있지 않을 때
            m_ObjectToHide.SetActive(m_Interactor.isActiveAndEnabled && !m_Interactor.hasSelection);

            // 필요에 따라 다음 로직을 사용할 수도 있습니다 (Ray Interactor가 사용 가능한 상태일 때):
            // m_ObjectToHide.SetActive(m_Interactor.isActiveAndEnabled);
        }
    }
}