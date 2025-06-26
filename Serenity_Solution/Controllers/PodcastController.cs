using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using Serenity_Solution.Models;
using System.Security.Claims;

namespace Serenity_Solution.Controllers
{
    public class PodcastController : Controller
    {
        private static List<PodcastViewModel> _podcasts = new List<PodcastViewModel>
        {
             new PodcastViewModel
            {
                Id = 1,
                Title = "Người ta có thương mình đâu",
                Description = "Có những tình cảm, mình biết ngay từ đầu là không thể.Vậy mà vẫn thương.Không đòi hỏi. Không hy vọng.Chỉ giữ trong lòng — như một điều đẹp nhất từng có. ",
                ImageUrl = "/image/Podcast/DeMemPhieuLuuKy.png",
                AudioUrl = "/Audio/NgTaKThuongMinh.mp3",
                Author = "Dế Mèn Du Ký",
                Duration = "20:54",
                Category = "Yêu thương & Kết nối",
                Rating = 4.8,
                RatingCount = 125
            },
            new PodcastViewModel
            {
                Id = 2,
                Title = "Bí quyết giúp con KHÔNG CÒN NHÚT NHÁT khi còn đi học",
                Description = "Xem phiên bản video:https://youtu.be/FY4qi0IUDtk?si=OFGhT-YXcrAOmpMY\r\n\r\nNhi Le Podcast cảm ơn bạn đã dành thời gian lắng nghe\r\n\r\n\r\nTheo dõi Fanpage tại https://www.facebook.com/AnneNhiLe và\r\n\r\n\r\nSubscribe kênh Youtube NHI LE https://www.youtube.com/@NHILE_SG để xem và cập nhật các kiến thức mới mỗi ngày",
                ImageUrl = "/image/Podcast/NhiLe.png",
                AudioUrl = "/Audio/BiQuyetGiupCon.mp3",
                Author = "Nhi Lê",
                Duration = "11:38",
                Category = "Tâm lý & Cảm xúc",
                Rating = 4.5,
                RatingCount = 98
            },
            new PodcastViewModel
            {
                Id = 3,
                Title = "1%",
                Description = "Không phải bỏ cuộc, cũng không phải tạm dừng, nhưng có nghĩa là không ngừng cố gắng. #bettereveryday (ảnh cover từ concert Coldplay hồi tháng 2 mà quên không khoe trong podcast T_T)",
                ImageUrl = "/image/podcast/MayKeChuyen.png",
                AudioUrl = "/Audio/1%.mp3",
                Author = "Mây Kể Chuyện",
                Duration = "11:53",
                Category = "Trò chuyện chữa lành",
                Rating = 4.7,
                RatingCount = 142
            },
            new PodcastViewModel
            {
                Id = 4,
                Title = "Lời hồi đáp thanh xuân|Radio#28",
                Description = "Tháng 5 về, nắng bắt đầu rực rỡ hơn, ve bắt đầu râm ran gọi hè, những chùm bằng lăng tím biếc như gom cả bầu trời vào trong ký ức, cũng là lúc những các bạn học sinh cuối cấp bước đứng trước ngưỡng cửa quan trọng mang tên Đại học.",
                ImageUrl = "/image/podcast/ViSaoTheNhi.png",
                AudioUrl = "/Audio/LoiHoiDap.mp3",
                Author = "Vì Sao Thế Nhỉ",
                Duration = "14:50",
                Category = "Tâm lý & Cảm xúc",
                Rating = 4.9,
                RatingCount = 205
            },
            new PodcastViewModel
            {
                Id = 5,
                Title = "Ngày bão Kể Chuyện Bão Lòng",
                Description = "Khám phá những lời minh triết về cuộc sống từ các triết gia và nhà tâm lý học nổi tiếng, giúp bạn có cái nhìn sâu sắc hơn về cuộc sống.",
                ImageUrl = "/image/podcast/KeChoToiNghe.png",
                AudioUrl = "/Audio/CauCoThe.mp3",
                Author = "Vì sao thế nhỉ",
                Duration = "26:45",
                Category = "Phát triển bản thân",
                Rating = 4.6,
                RatingCount = 168
            },
            new PodcastViewModel
            {
                Id = 6,
                Title = "Buổi 15: Tùy Duyên - Nhảy múa cùng vũ trụ",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề Tùy Duyên - Nhảy múa cùng vũ trụ do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream Tay Phật trong tay con - Buổi 15.\r\nKính chúc Quý vị nhiều bình an và vững chãi.",
                ImageUrl = "/image/podcast/MinhNiem.png",
                AudioUrl = "/Audio/Buoi15.m4a",
                Author = "Minh Niệm",
                Duration = "58:12",
                Category = "Phát triển bản thân",
                Rating = 4.8,
                RatingCount = 183
            },
            new PodcastViewModel
            {
                Id = 7,

                Title = "Vì sao mình làm Podcast",
                Description = "Tình yêu không chỉ là cảm xúc, mà còn là sự đồng hành và hỗ trợ. Podcast này sẽ giúp bạn hiểu cách để thực sự đồng hành cùng người mình thương trong những lúc khó khăn.",
                ImageUrl = "/image/podcast/ViSaoLamPodcast.png",
                AudioUrl = "/Audio/ViSaoPod.mp3",
                Author = "Mây Kể Chuyện",
                Duration = "07:43",
                Category = "Yêu thương & Kết nối",
                Rating = 4.7,
                RatingCount = 156
            },
            new PodcastViewModel
            {
                Id = 8,

                Title = "Nhân quả và sức khỏe",
                Description = "Theo cách nhìn nhà Phật là mình bị ốm vì do mình đã từng làm hại đến sức khỏe người khác. Đấy! Mình bị ốm nặng vì mình đã từng làm hại nặng nề đến sức khỏe người khác. Mình bị ốm chết bởi vì mình đã không chỉ làm hại sức khỏe mà mình còn giết hại người khác. Nên là tất cả những cái bệnh tật đều đến từ chỗ đấy.",
                ImageUrl = "/image/podcast/TraDamTrongSuot.png",
                AudioUrl = "/Audio/SucKhoe.mp3",
                Author = "Trà Đàm Trong Suốt",
                Duration = "139:50",
                Category = "Phát triển bản thân",
                Rating = 4.5,
                RatingCount = 132
            },
            new PodcastViewModel
            {
                Id = 9,
                Title = "Life Update: Cuộc sống của mình sau Podcast",
                Description = "Cuộc đời không có sẵn hướng dẫn, nhưng chúng ta có thể học hỏi từ kinh nghiệm của người khác. Podcast này tập hợp những lời khuyên và hướng dẫn quý giá cho cuộc sống.",
                ImageUrl = "/image/podcast/ThePresent2.png",
                AudioUrl = "/Audio/LifeUpdate.mp3",
                Author = "The Present Writing",
                Duration = "33:40",
                Category = "Phát triển bản thân",
                Rating = 4.6,
                RatingCount = 175
            },
            new PodcastViewModel
            {
                Id = 10,
                Title = "Buổi 13: Hòa Trong Sẽ Thuận Ngoài",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề Hòa trong sẽ thuận ngoài do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream Tay Phật trong tay con - Buổi 13.",
                ImageUrl = "/image/podcast/MinhNiem13.png",
                AudioUrl = "/Audio/Buoi13.m4a",
                Author = "Minh Niệm",
                Duration = "60:24",
                Category = "Thiền & Chánh niệm",
                Rating = 4.9,
                RatingCount = 210
            },
            new PodcastViewModel
            {
                Id = 11,
                Title = "Buổi 11: Chấp nhận ánh sáng và bóng tối bên trong",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề Chấp nhận cả ánh sáng và bóng tối bên trong do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream Tay Phật trong tay con - Buổi 11.",
                ImageUrl = "/image/podcast/MinhNiem11.png",
                AudioUrl = "/Audio/Buoi11.m4a",
                Author = "Minh Niệm",
                Duration = "45:42",
                Category = "Phát triển bản thân",
                Rating = 4.7,
                RatingCount = 158
            },
            new PodcastViewModel
            {
                Id = 12,
                Title = "Mình bị mất kết nối",
                Description = "Thực ra mình đã ẩn số này khi thấy ổn hơn, nhưng mình quyết định republish vì cuộc sống không ngừng lại và cuộc sống của mình cũng vậy, không phải khi mình sáng suốt hơn thì mình sẽ không phạm sai lầm, quan trọng là chúng mình vẫn tiếp tục và chưa bỏ cuộc!\r\n*gắn link không được nên các bạn search tiêu đề này trên youtube nhé BTS speech at the United Nations",
                ImageUrl = "/image/podcast/MatKetNoi.png",
                AudioUrl = "/Audio/matketnoi.m4a",
                Author = "Mây Kể Chuyện",
                Duration = "08:07",
                Category = "Thiền & Chánh niệm",
                Rating = 4.8,
                RatingCount = 187
            }
        };

        // Dữ liệu mẫu cho comments
        private static List<CommentViewModel> _comments = new List<CommentViewModel>
        {
            new CommentViewModel
            {
                CommetId = 1,
                CommentContent = "Podcast rất hay và ý nghĩa! Cảm ơn tác giả đã chia sẻ những kiến thức quý báu.",
                UserName = "PhanPhuocThinh",
                UserAvatar = "/image/avatars/user1.jpg",
                DateComment = DateTime.Now.AddDays(-2),
                PodcastID = 1,
                Rating = 5,
                ParentCommentId = null
            },
            
            
            new CommentViewModel
            {
                CommetId = 4,
                CommentContent = "Tôi hoàn toàn đồng ý với bạn!",
                UserName = "PhanPhuocThinh",
                UserAvatar = "/image/avatars/user4.jpg",
                DateComment = DateTime.Now.AddHours(-3),
                PodcastID = 1,
                Rating = 5,
                ParentCommentId = 1
            }
        };

        // Dữ liệu mẫu cho ratings
        private static List<RatingViewModel> _ratings = new List<RatingViewModel>
        {
            new RatingViewModel
            {
                Id = 1,
                Stars = 5,
                UserName = "PhanPhuocThinh",
                UserAvatar = "/image/avatars/user1.jpg",
                CreatedAt = DateTime.Now.AddDays(-2),
                PodcastId = 1
            },
            
            new RatingViewModel
            {
                Id = 4,
                Stars = 5,
                UserName = "PhanPhuocThinh",
                UserAvatar = "/image/avatars/user4.jpg",
                CreatedAt = DateTime.Now.AddHours(-3),
                PodcastId = 1
            }
        };

        public IActionResult Index(string category = null, int page = 1, string search = null)
        {
            int pageSize = 8;
            var query = _podcasts.AsQueryable();

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Filter by search term if provided
            if (!string.IsNullOrEmpty(search))
            {

                query = query.Where(p => 
                    p.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase) || 

                    p.Description.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, System.StringComparison.OrdinalIgnoreCase));
            }

            // Get distinct categories for filter tags
            ViewBag.Categories = _podcasts.Select(p => p.Category).Distinct().ToList();

            
            // Get total count for pagination
            int totalItems = query.Count();
            int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            

            // Apply pagination
            var podcasts = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pagination data for view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;

            return View(podcasts);
        }


        public static List<PodcastViewModel> GetPodcasts()
        {
            return _podcasts;
        }

        public IActionResult Detail(int id = 1)
        {
            var podcast = _podcasts.FirstOrDefault(p => p.Id == id) ?? _podcasts.First();


            // Get comments for this podcast
            var comments = _comments.Where(c => c.PodcastID == id).OrderByDescending(c => c.DateComment).ToList();
            
            // Get ratings for this podcast
            var ratings = _ratings.Where(r => r.PodcastId == id).ToList();
            
            // Calculate average rating
            var averageRating = ratings.Any() ? ratings.Average(r => r.Stars) : 0;
            var ratingCount = ratings.Count;

            // Get related podcasts (same category but different ID)
            var relatedPodcasts = _podcasts
                .Where(p => p.Id != podcast.Id && p.Category == podcast.Category)
                .Take(4)
                .ToList();

            // If not enough related podcasts in the same category, add some from other categories
            if (relatedPodcasts.Count < 4)
            {
                var additionalPodcasts = _podcasts
                    .Where(p => p.Id != podcast.Id && p.Category != podcast.Category)
                    .Take(4 - relatedPodcasts.Count)
                    .ToList();

                relatedPodcasts.AddRange(additionalPodcasts);
            }

            ViewBag.RelatedPodcasts = relatedPodcasts;
            ViewBag.Comments = comments;
            ViewBag.Ratings = ratings;
            ViewBag.AverageRating = averageRating;
            ViewBag.RatingCount = ratingCount;

            return View(podcast);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddComment(CommentViewModel model)
        {
            // Debug: Log ModelState errors
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = $"Validation errors: {string.Join(", ", errors)}";
                return RedirectToAction("Detail", new { id = model.PodcastID });
            }

            // Check if CommentContent is null or empty
            if (string.IsNullOrWhiteSpace(model.CommentContent))
            {
                TempData["Error"] = "Vui lòng nhập nội dung comment";
                return RedirectToAction("Detail", new { id = model.PodcastID });
            }

            var newComment = new CommentViewModel
            {
                CommetId = _comments.Max(c => c.CommetId) + 1,
                CommentContent = model.CommentContent,
                UserId = User.Identity.Name ?? "anonymous",
                UserName = User.Identity.Name ?? "Người dùng",
                UserAvatar = "/image/avatars/default.jpg",
                DateComment = DateTime.Now,
                PodcastID = model.PodcastID,
                Rating = model.Rating,
                ParentCommentId = model.ParentCommentId,
                Replies = new List<CommentViewModel>(),
                ParentComment = null
            };

            _comments.Add(newComment);

            TempData["Success"] = "Comment đã được thêm thành công!";
            return RedirectToAction("Detail", new { id = model.PodcastID });
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddRating(RatingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng chọn số sao";
                return RedirectToAction("Detail", new { id = model.PodcastId });
            }

            // Check if user already rated this podcast
            var existingRating = _ratings.FirstOrDefault(r => r.PodcastId == model.PodcastId && r.UserName == User.Identity.Name);
            
            if (existingRating != null)
            {
                // Update existing rating
                existingRating.Stars = model.Stars;
                existingRating.CreatedAt = DateTime.Now;
            }
            else
            {
                // Add new rating
                var newRating = new RatingViewModel
                {
                    Id = _ratings.Max(r => r.Id) + 1,
                    Stars = model.Stars,
                    UserName = User.Identity.Name ?? "Người dùng",
                    UserAvatar = "/image/avatars/default.jpg",
                    CreatedAt = DateTime.Now,
                    PodcastId = model.PodcastId
                };

                _ratings.Add(newRating);
            }

            TempData["Success"] = "Đánh giá đã được cập nhật thành công!";
            return RedirectToAction("Detail", new { id = model.PodcastId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteComment(int commentId, int podcastId)
        {
            var comment = _comments.FirstOrDefault(c => c.CommetId == commentId && c.UserName == User.Identity.Name);
            
            if (comment != null)
            {
                // Also delete all replies to this comment
                var replies = _comments.Where(c => c.ParentCommentId == commentId).ToList();
                foreach (var reply in replies)
                {
                    _comments.Remove(reply);
                }
                
                _comments.Remove(comment);
                TempData["Success"] = "Comment đã được xóa thành công!";
            }
            else
            {
                TempData["Error"] = "Không thể xóa comment này";
            }

            return RedirectToAction("Detail", new { id = podcastId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddReply(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = $"Validation errors: {string.Join(", ", errors)}";
                return RedirectToAction("Detail", new { id = model.PodcastID });
            }

            if (string.IsNullOrWhiteSpace(model.CommentContent))
            {
                TempData["Error"] = "Vui lòng nhập nội dung reply";
                return RedirectToAction("Detail", new { id = model.PodcastID });
            }

            var newReply = new CommentViewModel
            {
                CommetId = _comments.Max(c => c.CommetId) + 1,
                CommentContent = model.CommentContent,
                UserId = User.Identity.Name ?? "anonymous",
                UserName = User.Identity.Name ?? "Người dùng",
                UserAvatar = "/image/avatars/default.jpg",
                DateComment = DateTime.Now,
                PodcastID = model.PodcastID,
                Rating = 0, // Replies don't have ratings
                ParentCommentId = model.ParentCommentId,
                Replies = new List<CommentViewModel>(),
                ParentComment = null
            };

            _comments.Add(newReply);

            TempData["Success"] = "Reply đã được thêm thành công!";
            return RedirectToAction("Detail", new { id = model.PodcastID });
        }
    }
}
