using Easy;
using UnityEngine;

namespace Editor.Build
{
    [CreateAssetMenu(fileName = "build", menuName = "Build/config", order = 0)]
    public class BuildConfig : ScriptableObject
    {
        [Header("微信开发者工具配置")]
        /// <summary>
        /// 微信开发者工具
        /// </summary>
        [SerializeField][Rename("微信开发者工具路径")]public string wxDeveloperToolPath = "C:\\Program Files (x86)\\Tencent\\微信web开发者工具";
        /// <summary>
        /// 二维码文件地址
        /// </summary>
        [HideInInspector][SerializeField][Rename("预览二维码存放路径")]public string qrCodePath = "Assets/Editor/Build/code.txt";
        
        [Header("飞书机器人配置")]
        /// <summary>
        /// 飞书后台添加的机器人ID
        /// </summary>
        [SerializeField][Rename("机器人ID")] public string appid = "cli_a636a4f9e2b25013";
        /// <summary>
        /// 飞书后台添加的机器人密钥
        /// </summary>
        [SerializeField][Rename("机器人密钥")]public string appsecret = "nxIA7UOF1FBKJPJlojqHYfyJJ6nMH6Q4";
        /// <summary>
        /// 群里添加的机器人ID
        /// </summary>
        [SerializeField][Rename("群机器人hook")]public string webhook = "9b45ee32-9cb6-4422-a862-7134fbab731d";
        /// <summary>
        /// 群ID,可以在飞书开放平台的消息管理发送消息中获取
        /// </summary>
        [SerializeField][Rename("群ID")]public string groupID = "oc_01d4c7ce9c304994bdbfc70f626bcacf";

        [SerializeField] [Rename("凭证请求地址")]
        public string accessUrl = "https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal";
        [SerializeField] [Rename("飞书图片上传地址")]
        public string imgUploadUrl = "https://open.feishu.cn/open-apis/im/v1/images";

        [SerializeField] [Rename("机器人hook")]public string hookUrl = "https://open.feishu.cn/open-apis/bot/v2/hook/";

    }
}