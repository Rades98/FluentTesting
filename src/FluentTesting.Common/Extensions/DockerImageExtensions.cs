using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Testing.Common.Images;

namespace Testing.Common.Extensions
{
	/// <summary>
	/// Docker image extensions
	/// </summary>
	public static class DockerImageExtensions
	{
		private const string defaultImageSource = "hub.docker.com";

		/// <summary>
		/// With proxied image - use image behind harbor to not waste pull limit on images
		/// </summary>
		/// <typeparam name="TBuilderEntity"></typeparam>
		/// <typeparam name="TContainerEntity"></typeparam>
		/// <typeparam name="TConfigurationEntity"></typeparam>
		/// <param name="builderEntity">container builder</param>
		/// <param name="image">image name with version</param>
		/// <param name="useProxiedImages">True to use proxied images</param>
		/// <param name="imageSource">original docker registry with image, default is hub.docker.com</param>
		/// <returns></returns>
		public static TBuilderEntity WithProxiedImage<TBuilderEntity, TContainerEntity, TConfigurationEntity>(this TBuilderEntity builderEntity, string image,
																											bool useProxiedImages, string imageSource = defaultImageSource)
			where TBuilderEntity : ContainerBuilder<TBuilderEntity, TContainerEntity, TConfigurationEntity>
			where TContainerEntity : IContainer
			where TConfigurationEntity : IContainerConfiguration
		{
			builderEntity.WithImage(GetProxiedImagePath(image, useProxiedImages, imageSource));

			return builderEntity;
		}

		/// <summary>
		/// Get proxied image path - use image behind harbor to not waste pull limit on images
		/// </summary>
		/// <param name="image"></param>
		/// <param name="useProxiedImage"></param>
		/// <param name="imageSource"></param>
		/// <returns></returns>
		public static string GetProxiedImagePath(this string image, bool useProxiedImage, string? imageSource = defaultImageSource)
		{
			return useProxiedImage ? $"{ImagesSetting.ImageProxyPath}/{imageSource}/{image}" : $"{(!imageSource!.Equals(defaultImageSource) || useProxiedImage ? imageSource + "/" : "")}/{image}";
		}
	}
}
